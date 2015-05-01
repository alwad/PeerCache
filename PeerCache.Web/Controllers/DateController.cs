using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web.Http;

namespace PeerCache.Web.Controllers
{
    [RoutePrefix("api/date")]
    public class DateController : ApiController
    {
        [Route("now")]
        [HttpGet]
        //[OutputCache(5)]
        public IHttpActionResult Now()
        {
            var cache = PeerCache.PeerMemoryCache.Default;

            cache.Add<string>("test", "test", new CacheItemPolicy() { AbsoluteExpiration = DateTime.Now.AddSeconds(5) });
            cache.Remove("test");

            return Ok(DateTime.Now);
        }
    }
}
