using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Application.Cache
{
    public interface ICacheService
    {
        T Get<T>(string key);

        void Set<T>(string key, T value, int ttl);
    }
}
