using System.Collections.Generic;
using DashFire.Framework.Constants;

namespace DashFire.Framework.Models
{
    public class RegistrationModel
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

        public string SystemName
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

        public bool RegistrationRequired
        {
            get;
            set;
        }

        public JobExecutionMode JobExecutionMode
        {
            get;
            set;
        }

        public string OriginalInstanceId
        {
            get;
            set;
        }
    }
}
