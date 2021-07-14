using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace DashFire
{
    public class JobContext
    {
        internal static JobContext Instance = new JobContext();

        private List<Type> _jobTypes = new List<Type>();
        private List<IJob> _jobs = new List<IJob>();
        private IServiceProvider _serviceProvider;

        internal IEnumerable<IJob> Jobs => _jobs;

        internal IServiceProvider ServiceProvider => _serviceProvider;

        internal void RegisterJob<T>() where T : IJob
        {
            _jobTypes.Add(typeof(T));
        }

        internal void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var logger = (ILogger<JobContext>)serviceProvider.GetService(typeof(ILogger<JobContext>));

            foreach (var jobType in _jobTypes)
            {
                logger.LogInformation($"Initializing { jobType.FullName }");

                var constructors = jobType.GetConstructors();
                var constructor = constructors.Single();
                var parameters = new List<object>();
                foreach (var param in constructor.GetParameters())
                {
                    var service = serviceProvider.GetService(param.ParameterType);
                    parameters.Add(service);
                }

                var jobInstance = (IJob)Activator.CreateInstance(jobType, parameters.ToArray());
                _jobs.Add(jobInstance);
            }
        }
    }
}
