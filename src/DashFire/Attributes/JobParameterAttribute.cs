using System;

namespace DashFire.Attributes
{
    /// <summary>
    /// Job parameter attribute which is used to show the property is a job parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class JobParameterAttribute : Attribute
    {

        /// <summary>
        /// Parameter's display name
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Parameter's description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JobParameterAttribute()
        {
        }

        /// <summary>
        /// Constructor to set the display name of the parameter.
        /// </summary>
        /// <param name="displayName">Display name</param>
        public JobParameterAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        /// <summary>
        /// Constructor to set the display name and description of the parameter.
        /// </summary>
        /// <param name="displayName">Display name</param>
        /// <param name="description">Description</param>
        public JobParameterAttribute(string displayName, string description)
        {
            DisplayName = displayName;
            Description = description;
        }
    }
}
