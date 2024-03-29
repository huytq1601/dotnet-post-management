﻿using Microsoft.Extensions.Caching.Memory;
using PostManagement.Application.Interfaces;
using System.Collections.Concurrent;

namespace PostManagement.Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private ConcurrentDictionary<string, byte> _keys;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
            _keys = new ConcurrentDictionary<string, byte>();
        }

        public void SetWithKeyTracking(string key, object value)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(45))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                    .SetPriority(CacheItemPriority.Normal);
            _cache.Set(key, value, cacheEntryOptions);
            _keys.TryAdd(key, 0);
        }

        public void RemoveByPrefix(string prefix)
        {
            foreach (var key in _keys.Keys)
            {
                if (key.StartsWith(prefix))
                {
                    Remove(key);
                }
            }
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            byte temp;
            _keys.TryRemove(key, out temp);
        }
    }
}
