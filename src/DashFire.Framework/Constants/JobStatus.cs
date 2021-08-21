namespace DashFire.Framework.Constants
{
    /// <summary>
    /// Job's status.
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// Job has not run already.
        /// </summary>
        New,
        /// <summary>
        /// Job is registering itself at the server.
        /// </summary>
        Registering,
        /// <summary>
        /// Job is synchronizing using heart-bit mechanism.
        /// </summary>
        Synchronizing,
        /// <summary>
        /// Job has been scheduled and is in idle mode.
        /// </summary>
        Scheduled,
        /// <summary>
        /// Job is running.
        /// </summary>
        Running,
        /// <summary>
        /// When job is paused paused for some reason.
        /// </summary>
        Idle,
        /// <summary>
        /// Job has been shutdown.
        /// </summary>
        Shutdown,
        /// <summary>
        /// Job is crashed and could not continue.
        /// </summary>
        Crashed
    }
}
