using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DashFire
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;

            _logger.LogInformation("Initializing Jobs ...");
            JobContext.Instance.Initialize(serviceProvider);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Jobs ...");

            var tasks = new List<Task>();
            foreach (var job in JobContext.Instance.Jobs)
            {
                _logger.LogInformation($"Starting { job.JobInstance.JobInformation.Key }");
                tasks.Add(job.JobInstance.StartAsync(cancellationToken));
            }
            Task.WaitAll(tasks.ToArray(), cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Jobs ...");

            var tasks = new List<Task>();
            foreach (var job in JobContext.Instance.Jobs)
            {
                _logger.LogInformation($"Stopping { job.JobInstance.JobInformation.Key }");
                tasks.Add(job.JobInstance.ShutdownAsync(cancellationToken));
            }
            Task.WaitAll(tasks.ToArray(), cancellationToken);

            return Task.CompletedTask;
        }
    }
}
