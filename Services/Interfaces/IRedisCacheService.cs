using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IRedisCacheService<T> where T : class
    {
        Task<T?> GetCacheValueAsync(string key);

        Task SetCacheValueAsync(string key, T cacheValue);

        Task<T?> GetOrSetCacheAsync(string cacheKey, Func<Task<T>> fetchFromDataSource, DistributedCacheEntryOptions? options = default);
    }
}
