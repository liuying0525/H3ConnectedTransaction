using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OThinker.H3.Controllers.Startup))]
namespace OThinker.H3.Controllers
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // ConfigureAuth(app);
        }
    }
}