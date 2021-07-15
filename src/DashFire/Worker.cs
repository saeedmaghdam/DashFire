using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DashFire
{
    /// <summary>
    /// Worker which starts the jobs and manage the schedules.
    /// </summary>
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;

            _logger.LogInformation("");
            JobContext.Instance.Initialize(serviceProvider);
        }

        /// <summary>
        /// Start the service and the jobs as well.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a task.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Jobs ...");

            var tasks = new List<Task>();
            foreach (var job in JobContext.Instance.Jobs)
            {
                _logger.LogInformation($"Starting { job.JobInstance.JobInformation.SystemName }");
                tasks.Add(job.JobInstance.StartAsync(cancellationToken));
            }
            Task.WaitAll(tasks.ToArray(), cancellationToken);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Shutdown the jobs and stop the service.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a task.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Jobs ...");

            var tasks = new List<Task>();
            foreach (var job in JobContext.Instance.Jobs)
            {
                _logger.LogInformation($"Stopping { job.JobInstance.JobInformation.SystemName }");
                tasks.Add(job.JobInstance.ShutdownAsync(cancellationToken));
            }
            Task.WaitAll(tasks.ToArray(), cancellationToken);

            return Task.CompletedTask;
        }
    }
}
