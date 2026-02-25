using System.Web.Mvc;

using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

using MyApp.Infrastructure;
using MyApp.Business;
using MyApp.Web.Portal.Controllers;
using MyApp.Web.Portal.Helpers;

namespace MyApp.Web.Portal
{
    public partial class Startup
    {
        public void ConfigureServices()
        {
            var services = new ServiceCollection();
            RegisterServices(services);

            var resolver = new DefaultDependencyResolver(services.BuildServiceProvider());
            DependencyResolver.SetResolver(resolver);
        }

        private void RegisterServices(ServiceCollection services)
        {
            RegisterUtils(services);
            DependencyInjectionHelper.RegisterRepositories(services);
            DependencyInjectionHelper.RegisterBusiness(services);
            RegisterControllers(services);
        }

        private void RegisterUtils(ServiceCollection services)
        {
            services.AddAutoMapper(
                typeof(Startup).Assembly, 
                typeof(UsersBusiness).Assembly
            );
        }

        private void RegisterControllers(ServiceCollection services)
        {
            services.AddTransient<AccountController>();
            services.AddTransient<HomeController>();
            services.AddTransient<UsersController>();
        }
    }
}