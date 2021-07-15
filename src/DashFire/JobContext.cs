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

                // Get constructor parameters and inject dependency
                var constructors = jobType.GetConstructors();
                var constructor = constructors.Single();
                var parameters = new List<object>();
                foreach (var param in constructor.GetParameters())
                {
                    var service = serviceProvider.GetService(param.ParameterType);
                    parameters.Add(service);
                }

                // Generate job instance
                var jobInstance = (IJob)Activator.CreateInstance(jobType, parameters.ToArray());

                // Generate job parameters
                var parameterContainers = new List<JobParameterContainer>();
                var properties = jobType.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(JobParameterAttribute)));
                foreach (var property in properties)
                {
                    var attributes = property.GetCustomAttributes(typeof(JobParameterAttribute), true);
                    if (attributes.Any())
                    {
                        JobParameterAttribute attribute = (JobParameterAttribute)attributes.Single();
                        parameterContainers.Add(new JobParameterContainer()
                        {
                            ParameterName = property.Name,
                            DisplayName = attribute.DisplayName ?? property.Name,
                            Description = attribute.Description ?? string.Empty,
                            Type = property.PropertyType
                        });
                    }
                }

                // Create a job container and add to containers list
                _jobs.Add(new JobContainer()
                {
                    FullName = jobType.FullName,
                    JobType = jobType,
                    JobInstance = jobInstance,
                    Parameters = parameterContainers
                });
            }
        }
    }
}
