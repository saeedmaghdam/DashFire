namespace DashFire.Framework.Models
{
    /// <summary>
    /// Log job status model.
    /// </summary>
    public class LogJobStatusModel
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
        /// Log status message.
        /// </summary>
        public string Message
        {
            get;
            set;
        }
    }
}
