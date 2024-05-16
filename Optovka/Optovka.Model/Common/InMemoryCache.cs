using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace Optovka.Model
{
    public class InMemoryCache
    {
        private MemoryCache _cache;
        private CacheItemPolicy _policy;

        public InMemoryCache()
        {
            _cache = MemoryCache.Default;
            _policy = new CacheItemPolicy
            {
                // Set your expiration TimeSpan based on your needs
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
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
