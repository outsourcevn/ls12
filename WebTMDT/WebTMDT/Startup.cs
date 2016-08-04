using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebTMDT.Startup))]
namespace WebTMDT
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
