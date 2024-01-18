﻿using Microsoft.Extensions.Caching.Memory;

namespace EnityFrameworkRelationShip.Interfaces.Service
{
    public interface ICacheService
    {
        void SetWithKeyTracking(string key, object value);
        void RemoveByPrefix(string prefix);
        void Remove(string key);
    }
}