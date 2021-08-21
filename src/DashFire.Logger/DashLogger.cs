using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DashFire.Framework;
using DashFire.Framework.Constants;
using DashFire.Utils;
using Microsoft.Extensions.Logging;

namespace DashFire.Logger
{
    /// <summary>
    /// Handles DashFire level logs.
    /// </summary>
    public class DashLogger : IDashLogger
    {
        private readonly ILogger<DashLogger> _logger;
        private readonly QueueManager _queueManager;

        private Dictionary<string, bool> _registeredJobs = new Dictionary<string, bool>();

        /// <summary>
        /// Constructor of DashLogger.
        /// </summary>
        public DashLogger(ILogger<DashLogger> _logger, QueueManager queueManager)
        {
            _queueManager = queueManager;
        }

        /// <summary>
        /// Logs DashFire level logs and send to the Dashboard.
        /// </summary>
        /// <param name="key">Job's Key</param>
        /// <param name="instanceId">Job's instance id</param>
        /// <param name="message">Log message content</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task LogAsync(string key, string instanceId, string message, CancellationToken cancellationToken)
        {
            if (!_registeredJobs.ContainsKey($"{key}_{instanceId}"))
                return;

            var statusModel = new LogJobStatusModel()
            {
                Key = key,
                InstanceId = instanceId,
                Message = message
            };

            int remainingAttempts = 3;
            do
            {
                try
                {
                    _queueManager.Publish(MessageTypes.LogJobStatus, JsonSerializer.Serialize(statusModel));

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    remainingAttempts--;

                    await Task.Delay(100, cancellationToken);
                }
            } while (remainingAttempts != 0);
        }

        /// <summary>
        /// Register a job as registered.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="instanceId"></param>
        public void Register(string key, string instanceId)
        {
            if (ReflectionHelper.NameOfCallingClass() != "DashFire.Job")
                return;

            if (_registeredJobs.ContainsKey($"{key}_{instanceId}"))
                return;

            _registeredJobs.Add($"{key}_{instanceId}", true);
        }
    }
}
