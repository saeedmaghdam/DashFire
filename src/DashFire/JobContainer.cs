using System;
using System.Collections.Generic;

namespace DashFire
{
    public class JobContainer
    {
        public string Key
        {
            get;
            set;
        }

        public Type JobType
        {
            get;
            set;
        }

        public IJob JobInstance
        {
            get;
            set;
        }

        public List<JobParameterContainer> Parameters
        {
            get;
            set;
        }
    }
}
