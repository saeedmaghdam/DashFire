namespace DashFire.Models
{
    /// <summary>
    /// Job's parameter container structure.
    /// </summary>
    public class JobParameterModel
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        public string ParameterName
        {
            get;
            set;
        }

        /// <summary>
        /// Parameter type.
        /// </summary>
        public string TypeFullName
        {
            get;
            set;
        }

        /// <summary>
        /// Parameter display name.
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Parameter description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }
    }
}
