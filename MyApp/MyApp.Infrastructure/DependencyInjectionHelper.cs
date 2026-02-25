using Microsoft.Extensions.DependencyInjection;
using namasdev.Azure.Storage;
using namasdev.Net.Mail;
using Newtonsoft.Json;

using MyApp.Entities.Values;
using MyApp.Data;
using MyApp.Data.Sql;
using MyApp.Business;

namespace MyApp.Infrastructure
{
    public class DependencyInjectionHelper
    {
        public static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<SqlContext>();

            services.AddScoped<IParametersRepository, ParametersRepository>();
            services.AddScoped<FilesRepository>((sp) => new FilesRepository(sp.GetService<IParametersRepository>().GetCloudStorageAccount()));
            services.AddScoped<IErrorsRepository, ErrorsRepository>();
            services.AddScoped<IEmailsParametersRepository, EmailsParametersRepository>();
            services.AddScoped<IAspNetIdentityRepository, AspNetIdentityRepository>();
        }

        public static void RegisterBusiness(IServiceCollection services)
        {
            services.AddSingleton<EmailServerParameters>((sp) => JsonConvert.DeserializeObject<EmailServerParameters>(sp.GetService<IParametersRepository>().Get(Parameters.MAILS_SERVER)));

            services.AddScoped<IErrorsBusiness, ErrorsBusiness>();
            services.AddScoped<IEmailServer, EmailServer>();
            services.AddScoped<IEmailsBusiness, EmailsBusiness>();
            services.AddScoped<IUsersBusiness, UsersBusiness>();
        }
    }
}
