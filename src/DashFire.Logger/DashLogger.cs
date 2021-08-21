using System.Collections.Generic;
using System.Text.Json;
using DashFire.Framework;
using DashFire.Framework.Constants;
using DashFire.Utils;

namespace DashFire.Logger
{
    /// <summary>
    /// Handles DashFire level logs.
    /// </summary>
    public class DashLogger : IDashLogger
    {
        private readonly QueueManager _queueManager;

        private Dictionary<string, bool> _registeredJobs = new Dictionary<string, bool>();

        /// <summary>
        /// Constructor of DashLogger.
        /// </summary>
        public DashLogger(QueueManager queueManager)
        {
            _queueManager = queueManager;
        }

        /// <summary>
        /// Logs DashFire level logs and send to the Dashboard.
        /// </summary>
        /// <param name="key">Job's Key</param>
        /// <param name="instanceId">Job's instance id</param>
        /// <param name="message">Log message content</param>
        public void Log(string key, string instanceId, string message)
        {
            if (!_registeredJobs.ContainsKey($"{key}_{instanceId}"))
                return;

            var statusModel = new LogJobStatusModel()
            {
                Key = key,
                InstanceId = instanceId,
                Message = message
            };

            _queueManager.Publish(MessageTypes.LogJobStatus, JsonSerializer.Serialize(statusModel));
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
