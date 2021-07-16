using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace DashFire
{
    /// <summary>
    /// Jobs inherite this class in order to be recognized as a job.
    /// </summary>
    public abstract class Job : IJob
    {
        private readonly ILogger<Job> _logger;

        /// <summary>
        /// Contains job's information which will be used in whole system.
        /// </summary>
        public abstract JobInformation JobInformation
        {
            get;
        }

        /// <summary>
        /// Full name of the job.
        /// </summary>
        public string Key
        {
            get;
            internal set;
        }

        /// <summary>
        /// Job's instance id.
        /// </summary>
        public string InstanceId
        {
            get;
            internal set;
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
        protected Job()
        {
            _logger = (ILogger<Job>)JobContext.Instance.ServiceProvider.GetService(typeof(ILogger<Job>));
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
                // Register the job
                var registrationModel = new Models.RegistrationModel()
                {
                    Key = Key,
                    InstanceId = InstanceId,
                    Parameters = JobContext.Instance.Jobs.Single(x => x.Key == Key).Parameters.Select(x => new Models.JobParameterModel()
                    {
                        Description = x.Description,
                        DisplayName = x.DisplayName,
                        ParameterName = x.ParameterName,
                        TypeFullName = x.Type.FullName
                    }).ToList()
                };
                QueueManager.Instance.Publish(Constants.DashboardExchangeMessageTypes.Registration, JsonSerializer.Serialize(registrationModel));

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
