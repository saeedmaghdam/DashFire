using System.Collections.Generic;

namespace DashFire.Framework.Models
{
    /// <summary>
    /// Remote job execution model.
    /// </summary>
    public class JobExecutionRequestModel
    {
        /// <summary>
        /// Job's key.
        /// </summary>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Job's instance id.
        /// </summary>
        public string InstanceId
        {
            get;
            set;
        }

        /// <summary>
        /// New instance id which will be set to the job instance.
        /// </summary>
        public string NewInstanceId
        {
            get;
            set;
        }

        /// <summary>
        /// Job parameters.
        /// </summary>
        public IEnumerable<JobParameterValueModel> Parameters
        {
            get;
            set;
        }
    }
}
