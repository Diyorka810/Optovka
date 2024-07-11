using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;

namespace Optovka.Model
{
    public class InMemoryCache : IInMemoryCache
    {
        private MemoryCache _cache;
        private CacheItemPolicy _policy;
        private const int MinutesToLive = 5;

        public InMemoryCache()
        {
            _cache = MemoryCache.Default;
            _policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(MinutesToLive)
            };
        }

        public void Set(string key, object value)
        {
            _cache.Set(key, value, _policy);
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public T Get<T>(string key) where T : class
        {
            var item = _cache.Get(key) as T;
            return item;
        }

        public bool Contains(string key)
        {
            return _cache.Contains(key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}