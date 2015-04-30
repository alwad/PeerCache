using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace PeerCache
{
    public interface ICacheService 
    {
        void Add<T>(string key, T item, CacheItemPolicy cacheDetails);
        T Get<T>(string key);
        T Get<T>(string key, Func<string, T> notFoundCallback);
        T Get<T>(string key, Func<string, T> notFoundCallback, CacheItemPolicy cacheDetails);
        void Remove(string key);
    }
}
