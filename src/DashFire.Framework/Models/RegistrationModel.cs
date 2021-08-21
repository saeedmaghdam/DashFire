using System.Collections.Generic;
using DashFire.Framework.Constants;

namespace DashFire.Framework.Models
{
    /// <summary>
    /// Job registration model.
    /// </summary>
    public class RegistrationModel
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
        /// Job parameters.
        /// </summary>
        public List<JobParameterModel> Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// Job's system name.
        /// </summary>
        public string SystemName
        {
            get;
            set;
        }

        /// <summary>
        /// Job's display name.
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Job's description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Indicated whether registration is required or not.
        /// </summary>
        public bool RegistrationRequired
        {
            get;
            set;
        }

        /// <summary>
        /// Job's execution mode.
        /// </summary>
        public JobExecutionMode JobExecutionMode
        {
            get;
            set;
        }

        /// <summary>
        /// Job's original instance id.
        /// </summary>
        public string OriginalInstanceId
        {
            get;
            set;
        }
    }
}
