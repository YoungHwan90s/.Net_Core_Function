using Microsoft.Extensions.Caching.Distributed;
using NetCoreWebAPI.Interfaces.Services;
using System.Collections.Concurrent;
using System.Text.Json;

namespace NetCoreWebAPI.Services
{
    public class CacheService : ICacheService
    {

        private readonly IDistributedCache _distributedCache;
        private static readonly ConcurrentDictionary<string, bool> CacheKeys = new();

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            string? cachedValue = await _distributedCache.GetStringAsync(key);

            if (cachedValue == null)
            {
                return null;
            }

            T? value = JsonSerializer.Deserialize<T>(cachedValue);

            return value;
        }

        public async Task<T?> GetAsync<T>(string key, Func<Task<T>> factory) where T : class
        {
            T? cachedValue = await GetAsync<T>(key);

            if (cachedValue != null)
            {
                return cachedValue;
            }

            cachedValue = await factory();

            await SetAsync(key, cachedValue);

            return cachedValue;
        }

        public async Task SetAsync<T>(string key, T value) where T : class
        {
            string cachedValue = JsonSerializer.Serialize(value);

            await _distributedCache.SetStringAsync(key, cachedValue, new DistributedCacheEntryOptions
            {
                // AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(1),

            });

            CacheKeys.TryAdd(key, false);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);

            CacheKeys.TryRemove(key, out bool _);
        }

        public async Task RemoveByPrefixAsync(string prefixKey)
        {
            IEnumerable<Task> tasks = CacheKeys.Keys.Where(x => x.StartsWith(prefixKey)).Select(x => RemoveAsync(x));

            await Task.WhenAll(tasks);
        }
    }
}
