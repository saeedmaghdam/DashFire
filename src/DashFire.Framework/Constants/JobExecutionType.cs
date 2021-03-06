namespace DashFire.Framework.Constants
{
    /// <summary>
    /// Job execution type.
    /// </summary>
    public enum JobExecutionType
    {
        /// <summary>
        /// Job initialized as a service, it'll scheduled to be executed.
        /// </summary>
        Service,

        /// <summary>
        /// Job has been requested from server and it'll run once.
        /// </summary>
        Remote
    }
}
