namespace DashFire.Framework.Constants
{
    /// <summary>
    /// Job's registration status.
    /// </summary>
    public enum JobRegistrationStatus
    {
        /// <summary>
        /// Not initialized yet.
        /// </summary>
        New,
        /// <summary>
        /// Send a request and waiting for registration response.
        /// </summary>
        Registering,
        /// <summary>
        /// Registration done.
        /// </summary>
        Registered
    }
}
