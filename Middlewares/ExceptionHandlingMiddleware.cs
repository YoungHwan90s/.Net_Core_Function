using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreWebAPI.Data;
using NetCoreWebAPI.Data.Variable;
using NetCoreWebAPI.Models;

namespace NetCoreWebAPI.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly ApplicationDbContext _context;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                // Check Device
                bool isMobile = context.Request.Headers["User-Agent"].ToString().Contains(GlobalVariable.Mobile);
                string device = isMobile ? GlobalVariable.Mobile : GlobalVariable.Web;

                // Save path and message of the exception
                var newLogger = new ExceptionLogger
                {
                    Device = device,
                    ExceptionType = e.GetType().FullName,
                    StatusCode = context.Response.StatusCode,
                    Message = e.Message,
                    ErrorPath = e.StackTrace?.Substring(3, (e.StackTrace.IndexOf("line") + 7)),
                    CreatedAt = DateTime.Now,
                };

                _context.Entry(newLogger).State = EntityState.Added;
                _context.SaveChanges();

                _logger.LogError(e, "Exception occurred: {Message}", e.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Server Error",
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
                };

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(problemDetails);

                return;
            }
        }
    }
}
