using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace MyApp.AutomatedTasks
{
    internal partial class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            RegisterServices(builder.Services);

            using (IHost host = builder.Build())
            {
                await host.RunAsync();
            }
        }
    }
}
