

# DashFire
> A .Net library to create jobs and services which could connect to DashFire Dashboard easily.

DashFire is totally free and open source .Net library to create jobs and services, schedule, log and manage them easily.

When you create a Worker Service in .Net, or Windows Service in .Net Framework, you'll not only care about the logic of the business, but about logging, managing, scheduling and etc. 

But using DashFire, you only implement the logic of the business, the issues like communicating with the server (DashFire Dashboard), logging, scheduling and other sort of issues will be handled by the DashFire.
Then, you can host the jobs in Services (in Windows) or Systemd (in Linux based operation systems).

### P.S. This library is an alternative to HangFire which is a professional task scheduling library used by million of people around the world. This project is not as generic as HangFire, but I wanted to make a fully customized task scheduler. This is a very fundamental version which handles the basics of communications and scheduling, for extending the library and server, you can easily fork the project and extend it.
	

## Installation

To install DashFire in Visual Studio's Package Manager Console:

```sh

Install-Package DashFire -Version 0.1.0-beta

```

To install in a specific project use:

```sh

Install-Package DashFire -Version 0.1.0-beta -ProjectName Your_Project_Name

```

To update package use:

```sh

Update-Package DashFire

```

To update package in a specific project use:

```sh

Update-Package DashFire -ProjectName Your_Project_Name

```


Or visit DashFire's [Nuget][nuget-page] page to get more information.

## Usage example

1. First step is to add a job inheriting from Job and implement the required methods and properties:
```cs 
using System.Threading;
using System.Threading.Tasks;

namespace DashFire.Service.Sample
{
    public class SampleJob1 : Job
    {
        public override JobInformation JobInformation => throw new System.NotImplementedException();

        protected override Task StartInternallyAsync(CancellationToken cancellationToken) => throw new System.NotImplementedException();
    }
}

```
Dependency injection works as well.


2. Second step is to configure the worker:
In Program.cs, Add jobs to services, and use UseDashFire method in your host builder:
```cs
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddJob<SampleJob1>();
                })
                .UseDashFire();
```

3. Add required settings to appsettings.json:
```json
{
  "DashOptions": {
    "RabbitMqConnectionString": "amqp://guest:guest@localhost:5672/"
  },
  "SampleJob1": {
    "InstanceId": "R4uXWDOBp0m-FzI4GVF3sQ"
  }
}
```

Sample appsettings.json is available at:

https://github.com/saeedmaghdam/DashFire/blob/master/samples/SampleJob/appsettings.json

That's it!
Now you can run the app and it's do the rest!


## Release History
  
### Visit [CHANGELOG.md] to see full change log history of DashFire

* 0.1.0-beta
	* Initialized the library
		* Handles dependency injection
		* Handles logging
		* Handles communications
		* Registering the job
		* Handles remote execution messages
		* Handles heart-bit
		* Handles remote execution

## Meta
Saeed Aghdam – [Linkedin][linkedin]

Distributed under the MIT license. See [``LICENSE``][github-license] for more information.

[https://github.com/saeedmaghdam/](https://github.com/saeedmaghdam/)

## Contributing

1. Fork it (<https://github.com/saeedmaghdam/DashFire/fork>)
2. Create your feature branch (`git checkout -b feature/your-branch-name`)
3. Commit your changes (`git commit -am 'Add a short message describes new feature'`)
4. Push to the branch (`git push origin feature/your-branch-name`)

5. Create a new Pull Request

<!-- Markdown link & img dfn's -->

[linkedin]:https://www.linkedin.com/in/saeedmaghdam/
[nuget-page]:https://www.nuget.org/packages/DashFire
[github]: https://github.com/saeedmaghdam/
[github-page]: https://github.com/saeedmaghdam/DashFire/
[github-license]: https://raw.githubusercontent.com/saeedmaghdam/DashFire/master/LICENSE
[CHANGELOG.md]: https://github.com/saeedmaghdam/DashFire/blob/master/CHANGELOG.md
[DashFire.Test]: https://github.com/saeedmaghdam/DashFire/tree/master/DashFire.Test