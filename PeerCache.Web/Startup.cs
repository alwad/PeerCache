using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PeerCache.Web.Startup))]
namespace PeerCache.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
