using System;
using System.Configuration;

using Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Host.SystemWeb;

using MyApp.Web.Portal.Models;

[assembly: OwinStartup(typeof(MyApp.Web.Portal.Startup))]
namespace MyApp.Web.Portal
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                },
                ExpireTimeSpan = ObtenerAuthenticationTimeout(),
                CookieManager = new SystemWebCookieManager()
            });
        }

        private TimeSpan ObtenerAuthenticationTimeout()
        {
            return TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["AuthenticationTimeoutEnMin"]));
        }
    }
}