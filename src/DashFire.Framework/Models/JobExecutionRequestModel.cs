using System.Collections.Generic;

namespace DashFire.Framework.Models
{
    public class JobExecutionRequestModel
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

        public string NewInstanceId
        {
            get;
            set;
        }

        public IEnumerable<JobParameterValueModel> Parameters
        {
            get;
            set;
        }
    }
}
