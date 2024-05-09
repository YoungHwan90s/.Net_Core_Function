using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using NetCoreWebAPI.Data;
using NetCoreWebAPI.Interfaces.Repositories;
using NetCoreWebAPI.Interfaces.Services;
using NetCoreWebAPI.Middlewares;
using NetCoreWebAPI.Repositories;
using NetCoreWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISuperHeroRepository, SuperHeroRepository>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
//builder.Services.AddSingleton<ICacheService, CacheService>();

// Rate limiting settings
builder.Services.AddRateLimiter(options => 
{
    options.OnRejected = (context, _) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int) retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");

        return new ValueTask();
    };

    options.AddPolicy("Fixed", httpContext => 
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 3,
            Window = TimeSpan.FromSeconds(5)
        }));

    options.AddPolicy("Sliding", httpContext => 
    RateLimitPartition.GetSlidingWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new SlidingWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromSeconds(10),
            SegmentsPerWindow = 2
        }));

    options.AddPolicy("Token", httpContext => 
    RateLimitPartition.GetTokenBucketLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 100,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 5,
            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
            TokensPerPeriod = 20,
            AutoReplenishment = true
        }));

    options.AddPolicy("Concurrency", httpContext => 
    RateLimitPartition.GetConcurrencyLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new ConcurrencyLimiterOptions
        {
            PermitLimit  = 10,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 5
        }));
});

// DB Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("DB connection string is null or empty.");
    }

    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Distributed Cache - Redis
//builder.Services.AddStackExchangeRedisCache(option =>
//{
//    var redisConnection = builder.Configuration.GetValue<string>("AzureRedisConnection");

//    if (string.IsNullOrEmpty(redisConnection))
//    {
//        throw new InvalidOperationException("Redis connection string is null or empty.");
//    }
//    option.Configuration = redisConnection;
//});

var app = builder.Build();

app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
        .ExecuteAsync(statusCodeContext.HttpContext));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRateLimiter();

app.UseHttpsRedirection();

// Minimal Api
app.MapGet("/", () => "Minimal api test").WithName("Get hello!").RequireRateLimiting("Fixed");

app.UseAuthorization();

app.MapControllers();

app.Run();
