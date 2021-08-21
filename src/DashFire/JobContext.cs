using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using DashFire.Framework.Constants;

namespace DashFire
{
    /// <summary>
    /// Job's context.
    /// </summary>
    public class JobContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobContext> _logger;

        private readonly List<JobContainer> _jobs = new List<JobContainer>();

        internal IEnumerable<JobContainer> Jobs => _jobs;

        internal IEnumerable<JobContainer> ServiceJobs => _jobs.Where(x => x.ExecutionType == JobExecutionType.Service);

        internal IEnumerable<JobContainer> RemoteJobs => _jobs.Where(x => x.ExecutionType == JobExecutionType.Remote);

        /// <summary>
        /// Job context's constructor.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="logger">Logger instance.</param>
        public JobContext(IServiceProvider serviceProvider, ILogger<JobContext> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Register a new job in job context.
        /// </summary>
        /// <param name="jobType">Job's type.</param>
        internal void Register(Type jobType)
        {
            _logger.LogInformation($"Initializing { jobType.Name }");

            _jobs.Add(JobContainerHelper.BuildContainer(jobType, JobExecutionType.Service, _serviceProvider));
        }
    }
}
