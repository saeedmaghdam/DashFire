namespace DashFire.Framework.Models
{
    public class StatusModel
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

        public Constants.JobStatus JobStatus
        {
            get;
            set;
        }
    }
}
