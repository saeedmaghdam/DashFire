using System;

namespace DashFire.Models
{
    internal class JobScheduleModel
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
