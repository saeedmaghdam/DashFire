﻿using System;
using System.Collections.Generic;
using System.Linq;
using DashFire.Attributes;

namespace DashFire
{
    /// <summary>
    /// This class contains helper methods to work with job containers.
    /// </summary>
    internal static class JobContainerHelper
    {
        /// <summary>
        /// Create a job container contains job key, job type, job instance, job parameters and etc.
        /// </summary>
        /// <param name="jobType">Job's type.</param>
        /// <param name="serviceProvider">Service provider.</param>
        /// <returns>Returns a job container.</returns>
        internal static JobContainer BuildContainer(Type jobType, IServiceProvider serviceProvider)
        {
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

            // Create a job container and return it
            return new JobContainer()
            {
                Key = $"{jobType.FullName}:{jobType.Assembly.GetName().Version}",
                JobType = jobType,
                JobInstance = jobInstance,
                Parameters = parameterContainers
            };
        }
    }
}
