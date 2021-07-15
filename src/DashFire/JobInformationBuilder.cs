namespace DashFire
{
    /// <summary>
    /// Job information builder class
    /// </summary>
    public class JobInformationBuilder
    {
        private string _systemName;
        private string _displayName;
        private string _description;
        private string[] _cronSchedules;

        /// <summary>
        /// Sets a system name for the job.
        /// </summary>
        /// <param name="systemName">Job's system name</param>
        /// <returns>Job information builder</returns>
        public JobInformationBuilder SetSystemName(string systemName)
        {
            _systemName = systemName;

            return this;
        }

        /// <summary>
        /// Sets a display name for the job.
        /// </summary>
        /// <param name="displayName">Job's display name.</param>
        /// <returns>Job information builder</returns>
        public JobInformationBuilder SetDisplayName(string displayName)
        {
            _displayName = displayName;

            return this;
        }

        /// <summary>
        /// Sets a description for the job.
        /// </summary>
        /// <param name="description">Job's description.</param>
        /// <returns>Job information builder</returns>
        public JobInformationBuilder SetDescription(string description)
        {
            _description = description;

            return this;
        }

        /// <summary>
        /// Sets cron schedules for the job.
        /// </summary>
        /// <param name="cronSchedules">Job's cron schedules.</param>
        /// <returns>Job information builder</returns>
        public JobInformationBuilder SetCronSchedules(string[] cronSchedules)
        {
            _cronSchedules = cronSchedules;

            return this;
        }

        /// <summary>
        /// Creates an instance of JobInformationBuilder
        /// </summary>
        /// <returns></returns>
        public static JobInformationBuilder CreateInstance()
        {
            return new JobInformationBuilder();
        }

        /// <summary>
        /// Builds JobInformation using the parameters.
        /// </summary>
        /// <returns></returns>
        public JobInformation Build()
        {
            if (string.IsNullOrEmpty(_systemName))
                throw new System.Exception("System name is required.");

            return new JobInformation(_systemName, _displayName, _description, _cronSchedules);
        }
    }
}
