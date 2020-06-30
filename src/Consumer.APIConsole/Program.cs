using Consumer.Contracts.Interfaces;
using Consumer.OwnersFeed.Clients;
using Consumer.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Consumer.APIConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var services = ConfigureServices();
                var serviceProvider = services.BuildServiceProvider();

                var report = serviceProvider.GetService<OwnersReport>();
                await report.CreateReport();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.ToString()}");
            }
        }

        private static IServiceCollection ConfigureServices()
        {
            var builder = new ConfigurationBuilder()
                       .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                       .AddJsonFile("appsettings.json", optional: false);
            
            IConfigurationRoot configuration = builder.Build();


            IServiceCollection services = new ServiceCollection();
            services.AddTransient<IReporting, OwnersReport>();
            services.AddTransient<IReportOutput, ConsoleOutput>();
            services.AddHttpClient<IDataClient, DataClient>();
            services.AddSingleton<IConfiguration>(configuration);

            services.AddTransient<OwnersReport>();
            return services;
        }
    }
}
