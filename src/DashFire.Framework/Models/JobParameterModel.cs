namespace DashFire.Framework.Models
{
    /// <summary>
    /// Job parameter model.
    /// </summary>
    public class JobParameterModel
    {
        /// <summary>
        /// Parameter name.
        /// </summary>
        public string ParameterName
        {
            get;
            set;
        }

        /// <summary>
        /// Parameter type name.
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
