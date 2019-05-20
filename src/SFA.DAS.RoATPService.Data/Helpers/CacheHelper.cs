using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace SFA.DAS.RoATPService.Data.Helpers
{
    public class CacheHelper:ICacheHelper
    {
        private readonly MemoryCache _cache = MemoryCache.Default;
   
        public  List<T> Get<T>() 
        {
                var results = _cache.Get(typeof(T).Name);
                return (List<T>)results;
        }

        public void Cache<T>(IEnumerable<T> dataList, int minutesToCache)
        {
            _cache.Add(typeof(T).Name, dataList, new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, minutesToCache, 0) });
        }

        public void Cache<T>(IEnumerable<T> dataList)
        {
            _cache.Add(typeof(T).Name, dataList, new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, 10, 0) });
        }   

        public void PurgeAllCaches()
        {
            var cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            foreach (var cacheKey in cacheKeys)
            {
                MemoryCache.Default.Remove(cacheKey);
            }
        }
    }
}
