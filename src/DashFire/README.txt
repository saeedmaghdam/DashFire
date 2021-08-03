1. Create a class and inherit from Job (DashFire.Job):
    public class SampleJob1 : Job
    {
    }

2. Implement all methods and properties which are required:
    public override JobInformation JobInformation
    protected override async Task StartInternallyAsync(CancellationToken cancellationToken)

3. Create a host builder and add the job to the services in Program.cs:
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddJob<SampleJob1>();
                services.AddJob<SampleJob2>();
            });

4. Use method UseDashFire() in IHost:
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddJob<SampleJob1>();
                services.AddJob<SampleJob2>();
            })
            .UseDashFire();

5. Add instance id and rabbitmq connection string to appsettings.json:
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "DashOptions": {
    "RabbitMqConnectionString": "amqp://guest:guest@localhost:5672/"
  },
  "SampleJob1": {
    "InstanceId": "R4uXWDOBp0m-FzI4GVF3sQ"
  }
}

