using System;

namespace DashFire.Framework.Models
{
    /// <summary>
    /// Job schedule model.
    /// </summary>
    public class JobScheduleModel
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
        /// Job next execution model.
        /// </summary>
        public DateTime NextExecutionDateTime
        {
            get;
            set;
        }
    }
}
