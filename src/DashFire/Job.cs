using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DashFire.Framework;
using DashFire.Framework.Constants;
using DashFire.Framework.Models;
using DashFire.Utils;
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
        private readonly IDashLogger _dashLogger;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private JobRegistrationStatus _registrationStatus = JobRegistrationStatus.New;
        private HeartBitStatus _heartBitStatus = HeartBitStatus.New;
        private JobStatus _jobStatus = JobStatus.Idle;
        private long _heartBitExpirationTicks;
        private long _freshHeartBitExpirationTicks;
        private JobExecutionRequestModel _remoteExecutionRequest;

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

        internal JobExecutionMode JobExecutionMode
        {
            get;
            set;
        } = JobExecutionMode.ServiceMode;

        /// <summary>
        /// JobBase default constructor.
        /// </summary>
        protected Job()
        {
            _logger = (ILogger<Job>)Context.Instance.ServiceProvider.GetService(typeof(ILogger<Job>));
            _queueManager = (QueueManager)Context.Instance.ServiceProvider.GetService(typeof(QueueManager));
            _jobContext = (JobContext)Context.Instance.ServiceProvider.GetService(typeof(JobContext));
            _dashLogger = (IDashLogger)Context.Instance.ServiceProvider.GetService(typeof(IDashLogger));
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

            InitializeHeartBitAsync(cancellationToken);

            do
            {
                await RegisterJobAsync();
                await CheckServerAvailability();
                await ExecuteRequestAsync(cancellationToken);

                ChangeStatus(JobStatus.Running);
                _logger.LogInformation($"{JobInformation.SystemName} Started.");
                LogJobStatus("Job is running.");
                await StartInternallyAsync(cancellationToken);
                _logger.LogInformation($"{JobInformation.SystemName} Finished.");
                LogJobStatus("Job has been finished.");
                ChangeStatus(JobStatus.Idle);

                if (JobExecutionMode != JobExecutionMode.ServiceMode)
                    break;
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
            LogJobStatus("Job has been stopped.");

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
            ChangeStatus(JobStatus.Shutdown);
            _logger.LogInformation($"{JobInformation.SystemName} Shutdown!");
            LogJobStatus("Job has been shutdown.");

            await ShutdownInternallyAsync(cancellationToken);

            Shutdown();
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

        private async Task InitializeHeartBitAsync(CancellationToken cancellationToken)
        {
            await SendHeartBitAsync(cancellationToken: cancellationToken);
            do
            {
                await SendHeartBitAsync(TimeSpan.FromMinutes(1), cancellationToken);
            } while (!cancellationToken.IsCancellationRequested);
        }

        private async Task SendHeartBitAsync(TimeSpan delayTime = default(TimeSpan), CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_registrationStatus != JobRegistrationStatus.Registered)
            {
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
                return;
            }

            if (_heartBitStatus == HeartBitStatus.Requested)
            {
                if (DateTime.Now.Ticks < _freshHeartBitExpirationTicks)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
                    return;
                }
            }

            if (DateTime.Now.Ticks < _freshHeartBitExpirationTicks)
            {
                if (delayTime != default(TimeSpan))
                    await Task.Delay(delayTime, cancellationToken);
            }

            _heartBitStatus = HeartBitStatus.Requested;
            _freshHeartBitExpirationTicks = DateTime.Now.AddMinutes(2).Ticks;

            var heartBitModel = new HeartBitModel()
            {
                Key = Key,
                InstanceId = InstanceId
            };
            _queueManager.Publish(MessageTypes.HeartBit, JsonSerializer.Serialize(heartBitModel));
        }

        private async Task RegisterJobAsync()
        {
            if (_registrationStatus == JobRegistrationStatus.Registered)
                return;

            ChangeStatus(JobStatus.Registering);

            var registrationModel = new RegistrationModel()
            {
                Key = Key,
                InstanceId = InstanceId,
                Parameters = _jobContext.ServiceJobs.Single(x => x.Key == Key).Parameters.Select(x => new JobParameterModel()
                {
                    Description = x.Description,
                    DisplayName = x.DisplayName,
                    ParameterName = x.ParameterName,
                    TypeFullName = x.Type.FullName
                }).ToList(),
                SystemName = JobInformation.SystemName,
                Description = JobInformation.Description,
                DisplayName = JobInformation.DisplayName,
                RegistrationRequired = JobInformation.RegistrationRequired,
                JobExecutionMode = JobExecutionMode,
                OriginalInstanceId = _remoteExecutionRequest?.InstanceId
            };
            _registrationStatus = JobRegistrationStatus.Registering;
            _cancellationTokenSource = new CancellationTokenSource();

            _logger.LogInformation($"Registering job {JobInformation.SystemName} ...");

            _queueManager.Publish(MessageTypes.Registration, JsonSerializer.Serialize(registrationModel));

            _logger.LogInformation($"Registeration is required for job {JobInformation.SystemName}");

            if (JobInformation.RegistrationRequired)
            {
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

            _heartBitExpirationTicks = DateTime.Now.AddMinutes(1).Ticks;
        }

        private async Task CheckServerAvailability()
        {
            var isServerAlive = await IsServerAlive();
            if (isServerAlive)
                return;
            if (!JobInformation.RegistrationRequired)
                return;

            ChangeStatus(JobStatus.Synchronizing);

            do
            {
                _logger.LogError($"Server is not alive, execution of {JobInformation.SystemName} suspended.");
                await Task.Delay(TimeSpan.FromSeconds(3));

                isServerAlive = await IsServerAlive();
            } while (!isServerAlive);
        }

        private async Task ExecuteRequestAsync(CancellationToken cancellationToken)
        {
            if (_remoteExecutionRequest == null)
                return;

            if (_registrationStatus == JobRegistrationStatus.Registered)
            {
                // Create new instance and execute the instance and wait for the result!
                var currentJobContainer = _jobContext.Jobs.Where(x => x.Key == _remoteExecutionRequest.Key).Single();
                var jobContainer = JobContainerHelper.BuildContainer(currentJobContainer.JobType, JobExecutionType.Service, Context.Instance.ServiceProvider);
                var jobInstance = jobContainer.JobInstance as Job;
                jobInstance.InstanceId = _remoteExecutionRequest.NewInstanceId;
                jobInstance.JobExecutionMode = JobExecutionMode.ServerRequestedMode;
                jobInstance.JobInformation.RegistrationRequired = true;
                var properties = currentJobContainer.JobType.GetProperties();
                foreach (var newProperty in _remoteExecutionRequest.Parameters)
                {
                    var property = properties.Where(x => x.Name == newProperty.ParameterName).Single();
                    ReflectionHelper.SetPropertyValue(jobInstance, newProperty.ParameterName, newProperty.Value);
                }
                _queueManager.DeclareExchangeAndQueue(jobInstance.Key, jobInstance.InstanceId);

                await jobInstance.StartAsync(cancellationToken);
                await jobInstance.StopAsync(cancellationToken);
                await jobInstance.ShutdownAsync(cancellationToken);
            }

            _remoteExecutionRequest = null;
        }

        private Task<bool> IsServerAlive()
        {
            if (DateTime.Now.Ticks < _heartBitExpirationTicks)
                return Task.FromResult(true);

            _logger.LogWarning($"DashFire server is not alive.");
            return Task.FromResult(false);
        }

        private Task _queueManager_Received(string jobKey, string jobInstanceId, string messageType, string message)
        {
            if (jobKey != Key && jobInstanceId != InstanceId)
                return Task.CompletedTask;

            if (messageType == MessageTypes.Registration.ToString().ToLower())
            {
                _logger.LogInformation($"Job {JobInformation.SystemName} registered successfully.");
                LogJobStatus("Job has been registered successfully.");

                _registrationStatus = JobRegistrationStatus.Registered;
                if (_dashLogger != null)
                    _dashLogger.Register(Key, InstanceId);

                _cancellationTokenSource.Cancel();
            }
            else if (messageType == MessageTypes.HeartBit.ToString().ToLower())
            {
                _heartBitExpirationTicks = DateTime.Now.AddMinutes(1).Ticks;
                _heartBitStatus = HeartBitStatus.Alive;
            }
            else if (messageType == MessageTypes.JobExecutionRequest.ToString().ToLower())
            {
                var model = JsonSerializer.Deserialize<JobExecutionRequestModel>(message);
                _remoteExecutionRequest = model;
            }

            return Task.CompletedTask;
        }

        private async Task ScheduleAsync(CancellationToken cancellationToken)
        {
            if (JobInformation.CronSchedules.Any() && !cancellationToken.IsCancellationRequested)
            {
                ChangeStatus(JobStatus.Scheduled);

                NextExecutionDateTime = DateTime.MaxValue;
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
                    LogJobStatus($"Job has been scheduled to execute at {NextExecutionDateTime}.");

                    if (_registrationStatus == JobRegistrationStatus.Registered)
                    {
                        var jobScheduleModel = new JobScheduleModel()
                        {
                            Key = Key,
                            InstanceId = InstanceId,
                            NextExecutionDateTime = this.NextExecutionDateTime
                        };

                        _queueManager.Publish(MessageTypes.JobSchedule, JsonSerializer.Serialize(jobScheduleModel));
                    }

                    await Task.Delay(sleepTime, cancellationToken);
                }
            }
        }

        private void ChangeStatus(JobStatus jobStatus)
        {
            _jobStatus = jobStatus;

            if (_registrationStatus != JobRegistrationStatus.Registered)
                return;

            var statusModel = new StatusModel()
            {
                Key = Key,
                InstanceId = InstanceId,
                JobStatus = _jobStatus
            };

            _queueManager.Publish(MessageTypes.JobStatus, JsonSerializer.Serialize(statusModel));
        }

        private void LogJobStatus(string message)
        {
            if (_registrationStatus != JobRegistrationStatus.Registered)
                return;

            var statusModel = new LogJobStatusModel()
            {
                Key = Key,
                InstanceId = InstanceId,
                Message = message
            };

            _queueManager.Publish(MessageTypes.LogJobStatus, JsonSerializer.Serialize(statusModel));
        }

        private void Shutdown()
        {
            if (_registrationStatus != JobRegistrationStatus.Registered)
                return;

            var shutdownModel = new ShutdownModel()
            {
                Key = Key,
                InstanceId = InstanceId
            };

            _queueManager.Publish(MessageTypes.Shutdown, JsonSerializer.Serialize(shutdownModel));
        }
    }
}
