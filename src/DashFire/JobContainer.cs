using System;
using System.Collections.Generic;

namespace DashFire
{
    /// <summary>
    /// Contains the job including the job type, instances, parameters and some extra informations about the job.
    /// </summary>
    public class JobContainer
    {
        /// <summary>
        /// Full name of the job.
        /// </summary>
        public string FullName
        {
            get;
            set;
        }

        /// <summary>
        /// Job type.
        /// </summary>
        public Type JobType
        {
            get;
            set;
        }

        /// <summary>
        /// Job instance.
        /// </summary>
        public IJob JobInstance
        {
            get;
            set;
        }

        /// <summary>
        /// Parameters list including parameter name, display name, description and parameter type.
        /// </summary>
        public List<JobParameterContainer> Parameters
        {
            get;
            set;
        }
    }
}
