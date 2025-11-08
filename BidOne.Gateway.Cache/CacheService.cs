using BidOne.Gateway.Application.InfraInterfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BidOne.Gateway.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? ttl = null)
        {
            if (_cache.TryGetValue(key, out T cachedValue))
            {
                return cachedValue;
            }

            var value = await factory();

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl ?? TimeSpan.FromSeconds(30)
            };

            _cache.Set(key, value, cacheEntryOptions);

            return value;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
