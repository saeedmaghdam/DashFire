namespace DashFire.Framework.Models
{
    /// <summary>
    /// Job's status model.
    /// </summary>
    public class StatusModel
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
        /// Job's status.
        /// </summary>
        public Constants.JobStatus JobStatus
        {
            get;
            set;
        }
    }
}
