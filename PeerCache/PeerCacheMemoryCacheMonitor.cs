using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace PeerCache
{
    public class PeerCacheMemoryCacheMonitor : ChangeMonitor
    {
        private readonly string _cacheKey;
        private readonly string _uniqueId;

        public PeerCacheMemoryCacheMonitor(string cacheKey)
        {
            _uniqueId = Guid.NewGuid().ToString();
            _cacheKey = cacheKey;

        }

        public override string UniqueId 
        {
            get
            {
                return _uniqueId;
            }
        }

        protected override void Dispose(bool disposing)
        {
            
        }

        private void CacheItemChanged(object state)
        {
            Trace.TraceInformation("PeerCacheMemoryCacheMonitor received cache item notification. key: {0}, state {1}.",
                _cacheKey, state);
        }
    }
}
