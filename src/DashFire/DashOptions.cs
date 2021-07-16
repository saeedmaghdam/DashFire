namespace DashFire
{
    /// <summary>
    /// DashOptions which contains service options exists in appsettings.json.
    /// </summary>
    public class DashOptions
    {
        /// <summary>
        /// RabbitMq connection string.
        /// </summary>
        public string RabbitMqConnectionString
        {
            get;
            set;
        }
    }
}
