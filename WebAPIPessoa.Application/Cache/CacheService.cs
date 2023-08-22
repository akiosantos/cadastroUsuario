using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Application.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public T Get <T> (string key)
        {
            var cacheByte = _distributedCache.Get(key);
            if (cacheByte == null)
                return default (T);

            var cacheString = Encoding.UTF8.GetString (cacheByte);
            var resposta = JsonConvert.DeserializeObject<T> (cacheString);

            return resposta;
        }

        public void Set<T>(string key, T value, int ttl) // ttl - time to live
        {
            var cacheString = JsonConvert.SerializeObject (value);
            var cacheByte = Encoding.UTF8.GetBytes(cacheString);

            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(ttl));
            _distributedCache.Set(key, cacheByte, options);
        }
    }
}
