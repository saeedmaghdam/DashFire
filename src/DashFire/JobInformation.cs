namespace DashFire
{
    /// <summary>
    /// Job information structure.
    /// </summary>
    public sealed class JobInformation
    {
        /// <summary>
        /// Job's system name.
        /// </summary>
        public string SystemName
        {
            get;
        }

        /// <summary>
        /// Job's display name.
        /// </summary>
        public string DisplayName
        {
            get;
        }

        /// <summary>
        /// Job's description.
        /// </summary>
        public string Description
        {
            get;
        }

        /// <summary>
        /// Job's execution schedules in cron format.
        /// </summary>
        public string[] CronSchedules
        {
            get;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="systemName">Job's system name.</param>
        /// <param name="displayName">Job's display name.</param>
        /// <param name="description">Job's description.</param>
        /// <param name="cronSchedules">Job's cron schedules.</param>
        internal JobInformation(string systemName, string displayName, string description, string[] cronSchedules)
        {
            SystemName = systemName;
            DisplayName = displayName ?? systemName;
            Description = description;
            CronSchedules = cronSchedules ?? new string[] { };
        }
    }
}
