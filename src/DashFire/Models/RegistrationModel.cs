using System.Collections.Generic;

namespace DashFire.Models
{
    /// <summary>
    /// Job's registration model
    /// </summary>
    public class RegistrationModel
    {
        /// <summary>
        /// Full name of the job.
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
        /// Parameters list including parameter name, display name, description and parameter type.
        /// </summary>
        public List<JobParameterModel> Parameters
        {
            get;
            set;
        }
    }
}
