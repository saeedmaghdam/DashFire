using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace DashFire
{
    /// <summary>
    /// Job context which contains job type and instances, service provider.
    /// </summary>
    internal class Context
    {
        internal static Context Instance = new Context();

        private readonly List<Type> _jobTypes = new List<Type>();

        internal IServiceProvider ServiceProvider { get; private set; }

        internal void RegisterJob<T>() where T : IJob
        {
            _jobTypes.Add(typeof(T));
        }

        internal void Initialize(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            var jobContext = (JobContext)ServiceProvider.GetService(typeof(JobContext));

            foreach (var jobType in _jobTypes)
            {
                jobContext.Register(jobType);
            }
        }
    }
}
