using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace PeerCache
{
    public class PeerCacheItemPolicy : CacheItemPolicy
    {
        public delegate void PeerCacheChanged(string key, string reason);
        public PeerCacheChanged OnPeerCacheChanged { get; set; }

        private CacheEntryRemovedCallback InnerRemovedCallback { get; set; }


        public PeerCacheItemPolicy(CacheItemPolicy innerPolicy)
        {
            base.AbsoluteExpiration = innerPolicy.AbsoluteExpiration;
            base.Priority = innerPolicy.Priority;
            base.SlidingExpiration = innerPolicy.SlidingExpiration;

            foreach (var monitor in innerPolicy.ChangeMonitors)
            {
                base.ChangeMonitors.Add(monitor);
            }

            if (innerPolicy.UpdateCallback != null)
                throw new NotSupportedException("UpdateCallback not supported.");

            InnerRemovedCallback = innerPolicy.RemovedCallback;
            base.RemovedCallback = OnRemovedCallback;
        }

        private void OnRemovedCallback(CacheEntryRemovedArguments arguments)
        {
            if (InnerRemovedCallback != null)
            {
                InnerRemovedCallback(arguments);
            }

            if (OnPeerCacheChanged != null)
                OnPeerCacheChanged(arguments.CacheItem.Key, arguments.RemovedReason.ToString());
        }
    }
}
