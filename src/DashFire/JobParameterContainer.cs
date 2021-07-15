using System;
namespace DashFire
{
    /// <summary>
    /// Job's parameter container structure.
    /// </summary>
    public class JobParameterContainer
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
        public Type Type
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
