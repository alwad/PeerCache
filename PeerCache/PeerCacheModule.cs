using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;

namespace PeerCache
{
    public class PeerCacheModule : IHttpModule
    {
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            Trace.TraceInformation("Peer cache module is starting");

            //var cache = System.Runtime.Caching.MemoryCache

            
        }
    }
}
