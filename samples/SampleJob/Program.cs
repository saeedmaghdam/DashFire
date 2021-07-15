using DashFire;
using Microsoft.Extensions.Hosting;

namespace SampleJob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddJob<SampleJob1>();
                    services.AddJob<SampleJob2>();
                })
                .UseDashFire();
    }
}
