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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Jobs ...");

            var tasks = new List<Task>();
            foreach (var job in JobContext.Instance.Jobs)
            {
                _logger.LogInformation($"Starting { job.GetType().Name }");
                tasks.Add(job.StartAsync(cancellationToken));
            }
            Task.WaitAll(tasks.ToArray(), cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            foreach (var job in JobContext.Instance.Jobs)
                tasks.Add(job.ShutdownAsync(cancellationToken));
            Task.WaitAll(tasks.ToArray(), cancellationToken);

            return Task.CompletedTask;
        }
    }
}
