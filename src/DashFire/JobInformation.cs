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
        /// Indicated whether registration required or not. if registration required, job will not trigger til job register itself to central unit.
        /// </summary>
        public bool RegistrationRequired
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
        /// <param name="registrationRequired">Indicated whether registration is required or not.</param>
        internal JobInformation(string systemName, string displayName, string description, string[] cronSchedules, bool registrationRequired)
        {
            SystemName = systemName;
            DisplayName = displayName ?? systemName;
            Description = description;
            CronSchedules = cronSchedules ?? new string[] { };
            RegistrationRequired = registrationRequired;
        }
    }
}
