using System;

namespace DashFire.Framework.Models
{
    public class JobScheduleModel
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

        public DateTime NextExecutionDateTime
        {
            get;
            set;
        }
    }
}
