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
        private readonly QueueManager _queueManager;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="serviceProvider">Service provider</param>
        /// <param name="queueManager">Queue manager</param>
        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, QueueManager queueManager)
        {
            _logger = logger;
            _queueManager = queueManager;

            _logger.LogInformation("");
            JobContext.Instance.Initialize(serviceProvider);

            _queueManager.Received += _queueManager_Received;
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
                _logger.LogInformation($"Initializing the queue for { job.JobInstance.JobInformation.SystemName }");
                _queueManager.Initialize(job.JobInstance.Key, job.JobInstance.InstanceId);
                _queueManager.StartConsume(job.JobInstance.Key, job.JobInstance.InstanceId);

                _logger.LogInformation($"Starting { job.JobInstance.JobInformation.SystemName }");
                tasks.Add(job.JobInstance.StartAsync(cancellationToken));
            }

            try
            {
                Task.WaitAll(tasks.ToArray(), cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError("A cancellation signal has been thrown from the service.");
            }
            catch (AggregateException exs)
            {
                foreach (var exception in exs.InnerExceptions)
                {
                    _logger.LogError(exception.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

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

        private Task _queueManager_Received(string jobKey, string jobInstanceId, string messageType, string message)
        {
            return Task.CompletedTask;
        }
    }
}
