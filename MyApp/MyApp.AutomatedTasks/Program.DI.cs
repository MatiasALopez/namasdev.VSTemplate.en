using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

using MyApp.Business;
using MyApp.Infrastructure;

namespace MyApp.AutomatedTasks
{
    internal partial class Program
    {
        static void RegisterServices(IServiceCollection services)
        {
            RegisterUtils(services);
            DependencyInjectionHelper.RegisterRepositories(services);
            DependencyInjectionHelper.RegisterBusiness(services);
            RegisterAutomatedTasksServices(services);
        }

        static void RegisterUtils(IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(
                    typeof(Program),
                    typeof(UsersBusiness)
                );
            });

            services.AddSingleton<IMapper>(sp => config.CreateMapper());
        }

        static void RegisterAutomatedTasksServices(IServiceCollection services)
        {
            services.AddHostedService<AutomatedTasksService>();
        }
    }
}
