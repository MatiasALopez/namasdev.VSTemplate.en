using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyApp.Web.Portal.Startup))]
namespace MyApp.Web.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureServices();
        }
    }
}
