using System;
using System.Collections.Generic;
using System.Linq;
using DashFire.Attributes;
using Microsoft.Extensions.Logging;

namespace DashFire
{
    /// <summary>
    /// Job context which contains job type and instances, service provider.
    /// </summary>
    internal class JobContext
    {
        public static JobContext Instance = new JobContext();

        private readonly List<Type> _jobTypes = new List<Type>();
        private readonly List<JobContainer> _jobs = new List<JobContainer>();

        internal IEnumerable<JobContainer> Jobs => _jobs;

        internal IServiceProvider ServiceProvider { get; private set; }

        internal void RegisterJob<T>() where T : IJob
        {
            _jobTypes.Add(typeof(T));
        }

        internal void Initialize(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            var logger = (ILogger<JobContext>)serviceProvider.GetService(typeof(ILogger<JobContext>));

            foreach (var jobType in _jobTypes)
            {
                logger.LogInformation($"Initializing { jobType.Name }");

                // Create a job container and add to containers list
                _jobs.Add(JobContainerHelper.BuildContainer(jobType, serviceProvider));
            }
        }
    }
}
