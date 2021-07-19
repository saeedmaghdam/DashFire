using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DashFire
{
    /// <summary>
    /// Generic IHostBuilder extensions to use DashFire
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Use DashFire in current program.
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder to config to use DashFire</param>
        /// <returns>Config the IHostBuilder and return it.</returns>
        public static IHostBuilder UseDashFire(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true);
                config.AddEnvironmentVariables();
            }).ConfigureServices((hostContext, services) =>
            {
                services.AddOptions();

                var dashOptions = hostContext.Configuration.GetSection("DashOptions").GetChildren();
                services.Configure<DashOptions>(options => hostContext.Configuration.GetSection("DashOptions").Bind(options));

                services.AddSingleton<QueueManager>();
                services.AddSingleton<JobContext>();

                services.AddHostedService<Worker>();
            });

            return hostBuilder;
        }
    }
}
