using System;

namespace DashFire.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class JobParameterAttribute : Attribute
    {
        private string _name;
        private string _description;

        public string Name => _name;

        public string Description => _description;

        public JobParameterAttribute()
        {
        }

        public JobParameterAttribute(string name)
        {
            _name = name;
        }

        public JobParameterAttribute(string name, string description)
        {
            _name = name;
            _description = description;
        }
    }
}
