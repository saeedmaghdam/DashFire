using System;
using System.Collections.Generic;
using DashFire.Framework.Constants;

namespace DashFire
{
    /// <summary>
    /// Contains the job including the job type, instances, parameters and some extra informations about the job.
    /// </summary>
    internal class JobContainer
    {
        /// <summary>
        /// Full name of the job.
        /// </summary>
        internal string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Job type.
        /// </summary>
        internal Type JobType
        {
            get;
            set;
        }

        /// <summary>
        /// Job instance.
        /// </summary>
        internal IJob JobInstance
        {
            get;
            set;
        }

        /// <summary>
        /// Job execution type
        /// </summary>
        internal JobExecutionType ExecutionType
        {
            get;
            set;
        }

        /// <summary>
        /// Parameters list including parameter name, display name, description and parameter type.
        /// </summary>
        internal List<JobParameterContainer> Parameters
        {
            get;
            set;
        }
    }
}
