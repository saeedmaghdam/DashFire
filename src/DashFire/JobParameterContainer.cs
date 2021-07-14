using System;
namespace DashFire
{
    public class JobParameterContainer
    {
        public string ParameterName
        {
            get;
            set;
        }

        public Type Type
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
    }
}
