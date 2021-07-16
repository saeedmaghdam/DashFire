using System.Collections.Generic;

namespace DashFire.Models
{
    internal class RegistrationModel
    {
        public string Key
        {
            get;
            set;
        }

        public string InstanceId
        {
            get;
            set;
        }
        
        public List<JobParameterModel> Parameters
        {
            get;
            set;
        }
    }
}
