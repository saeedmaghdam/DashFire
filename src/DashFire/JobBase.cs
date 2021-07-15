using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace DashFire
{
    /// <summary>
    /// Jobs inherite this class in order to be recognized as a job.
    /// </summary>
    public abstract class JobBase : IJob
    {
        private readonly ILogger<JobBase> _logger;

        /// <summary>
        /// Contains job's information which will be used in whole system.
        /// </summary>
        public abstract JobInformation JobInformation
        {
            get;
        }

        /// <summary>
        /// Job's next execution date and time.
        /// </summary>
        public DateTime NextExecutionDateTime
        {
            get;
            private set;
        }

        /// <summary>
        /// JobBase default constructor.
        /// </summary>
        protected JobBase()
        {
            _logger = (ILogger<JobBase>)JobContext.Instance.ServiceProvider.GetService(typeof(ILogger<JobBase>));
        }

        /// <summary>
        /// Start the job.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a task.</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            do
            {
                _logger.LogInformation($"{JobInformation.SystemName} Started.");

                await StartInternallyAsync(cancellationToken);

                if (JobInformation.CronSchedules.Any() && !cancellationToken.IsCancellationRequested)
                {
                    var NextExecutionDateTime = DateTime.MaxValue;
                    foreach (var cronExpression in JobInformation.CronSchedules)
                    {
                        var crontabSchedule = CrontabSchedule.Parse(cronExpression);
                        var nextOccurrence = crontabSchedule.GetNextOccurrence(DateTime.Now, DateTime.MaxValue);
                        NextExecutionDateTime = NextExecutionDateTime < nextOccurrence ? NextExecutionDateTime : nextOccurrence;
                    }

                    var sleepTime = NextExecutionDateTime - DateTime.Now;
                    if (sleepTime.TotalSeconds > 0)
                    {
                        _logger.LogInformation($"{JobInformation.SystemName} scheduled to execute at {NextExecutionDateTime}");

                        await Task.Delay(sleepTime, cancellationToken);
                    }
                }
            } while (!cancellationToken.IsCancellationRequested);
        }

        /// <summary>
        /// Triggers whenever job starts.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a task.</returns>
        protected internal abstract Task StartInternallyAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Stop the job.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a task.</returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{JobInformation.SystemName} Stoped!");

            await StopInternallyAsync(cancellationToken);
        }

        /// <summary>
        /// Triggers whenever job stops.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a task.</returns>
        protected virtual Task StopInternallyAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when the service stops.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a task.</returns>
        public async Task ShutdownAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{JobInformation.SystemName} Shutdown!");

            await ShutdownInternallyAsync(cancellationToken);
        }

        /// <summary>
        /// Triggers whenever the service going to shutdown.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a task.</returns>
        protected virtual Task ShutdownInternallyAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
