using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DashFire
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseDashService(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true);
                config.AddEnvironmentVariables();

                //if (args != null)
                //{
                //    config.AddCommandLine(args);
                //}
            }).ConfigureServices((hostContext, services) =>
            {
                services.AddOptions();

                var dashOptions = hostContext.Configuration.GetSection("DashOptions").GetChildren();
                services.Configure<DashOptions>(options => hostContext.Configuration.GetSection("DashOptions").Bind(options));

                services.AddHostedService<Worker>();
            });

            return hostBuilder;
        }
    }
}
