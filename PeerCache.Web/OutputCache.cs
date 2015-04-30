using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;

using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace PeerCache.Web
{
    public class OutputCacheAttribute : ActionFilterAttribute
    {
        int _duration;

        /// <param name="duration">Time to cache in seconds</param>
        public OutputCacheAttribute(int duration)
        {
            if (duration <= 0)
                throw new InvalidOperationException("Invalid Duration");

            _duration = duration;
        }

        private static ICacheService Cache
        {
            get
            {
                return PeerMemoryCache.Default;
            }
        }

        private Action<HttpActionExecutedContext> Callback { set; get; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string _key = string.Join(":", new string[]
                        {
                            actionContext.Request.RequestUri.OriginalString,
                            "application/json"
                            //more keys here 
                        });

            if (actionContext == null)
            {
                throw new ArgumentNullException("actionExecutedContext");
            }

            string cachedValue = Cache.Get<string>(_key);
            if (cachedValue != null)
            {
                actionContext.Response = actionContext.Request.CreateResponse();
                actionContext.Response.Content = new StringContent(cachedValue);
                actionContext.Response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return;
            }
            Callback = (actionExecutedContext) =>
            {
                var content = actionExecutedContext.Response.Content;

                if (content != null && actionExecutedContext.Response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var output = content.ReadAsStringAsync().Result;
                    Cache.Add<string>(_key, output, new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(_duration) });
                }
            };
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext == null)
            {
                throw new ArgumentNullException("actionExecutedContext");
            }
            Callback(actionExecutedContext);
        }
    }
}