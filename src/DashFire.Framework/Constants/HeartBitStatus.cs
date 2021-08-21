namespace DashFire.Framework.Constants
{
    /// <summary>
    /// Heartbit status.
    /// </summary>
    public enum HeartBitStatus
    {
        /// <summary>
        /// Not initialized yet.
        /// </summary>
        New,
        /// <summary>
        /// Heartbit sent to server.
        /// </summary>
        Requested,
        /// <summary>
        /// Server is alive.
        /// </summary>
        Alive
    }
}
