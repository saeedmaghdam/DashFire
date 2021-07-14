using System;
using System.Collections.Generic;
using System.Linq;
using DashFire.Attributes;
using Microsoft.Extensions.Logging;

namespace DashFire
{
    public class JobContext
    {
        internal static JobContext Instance = new JobContext();

        private List<Type> _jobTypes = new List<Type>();
        private List<JobContainer> _jobs = new List<JobContainer>();
        private IServiceProvider _serviceProvider;

        internal IEnumerable<JobContainer> Jobs => _jobs;

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
                            DisplayName = attribute.Name != null ? attribute.Name : property.Name,
                            Description = attribute.Description != null ? attribute.Description : string.Empty,
                            Type = property.PropertyType
                        });
                    }
                }

                // Create a job container and add to containers list
                _jobs.Add(new JobContainer()
                {
                    Key = jobType.FullName,
                    JobType = jobType,
                    JobInstance = jobInstance,
                    Parameters = parameterContainers
                });
            }
        }
    }
}
