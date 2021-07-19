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
        private readonly QueueManager _queueManager;
        private readonly JobContext _jobContext;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Constants.JobRegistrationStatus _registrationStatus = Constants.JobRegistrationStatus.New;

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
            _logger = (ILogger<Job>)Context.Instance.ServiceProvider.GetService(typeof(ILogger<Job>));
            _queueManager = (QueueManager)Context.Instance.ServiceProvider.GetService(typeof(QueueManager));
            _jobContext = (JobContext)Context.Instance.ServiceProvider.GetService(typeof(JobContext));
        }

        /// <summary>
        /// Start the job.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a task.</returns>
        internal async Task StartAsync(CancellationToken cancellationToken)
        {
            _queueManager.Received += _queueManager_Received;
            _queueManager.StartConsume(Key, InstanceId);

            do
            {
                // Register the job
                await RegisterJobAsync();

                _logger.LogInformation($"{JobInformation.SystemName} Started.");
                await StartInternallyAsync(cancellationToken);
                _logger.LogInformation($"{JobInformation.SystemName} Finished.");

                await ScheduleAsync(cancellationToken);
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
        internal async Task StopAsync(CancellationToken cancellationToken)
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
        internal async Task ShutdownAsync(CancellationToken cancellationToken)
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

        private async Task RegisterJobAsync()
        {
            if (_registrationStatus == Constants.JobRegistrationStatus.Registered)
                return;

            var registrationModel = new Models.RegistrationModel()
            {
                Key = Key,
                InstanceId = InstanceId,
                Parameters = _jobContext.ServiceJobs.Single(x => x.Key == Key).Parameters.Select(x => new Models.JobParameterModel()
                {
                    Description = x.Description,
                    DisplayName = x.DisplayName,
                    ParameterName = x.ParameterName,
                    TypeFullName = x.Type.FullName
                }).ToList()
            };
            _registrationStatus = Constants.JobRegistrationStatus.Registering;
            _cancellationTokenSource = new CancellationTokenSource();

            _logger.LogInformation($"Registering job {JobInformation.SystemName} ...");

            _queueManager.Publish(Constants.MessageTypes.Registration, JsonSerializer.Serialize(registrationModel));

            _logger.LogInformation($"Registeration is required for job {JobInformation.SystemName}");

            do
            {
                try
                {
                    _logger.LogInformation($"Job {JobInformation.SystemName} is waiting for the registration response.");

                    await Task.Delay(int.MaxValue, _cancellationTokenSource.Token);
                }
                catch { }
            } while (!_cancellationTokenSource.IsCancellationRequested);
        }

        private Task _queueManager_Received(string jobKey, string jobInstanceId, string messageType, string message)
        {
            if (jobKey == Key && jobInstanceId == InstanceId && messageType == Constants.MessageTypes.Registration.ToString().ToLower())
            {
                _logger.LogInformation($"Job {JobInformation.SystemName} registered successfully.");

                _registrationStatus = Constants.JobRegistrationStatus.Registered;
                _cancellationTokenSource.Cancel();
            }

            return Task.CompletedTask;
        }

        private async Task ScheduleAsync(CancellationToken cancellationToken)
        {
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
        }
    }
}
