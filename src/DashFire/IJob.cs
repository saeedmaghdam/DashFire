using System.Threading;
using System.Threading.Tasks;

namespace DashFire
{
    /// <summary>
    /// Job's interface.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Job's information.
        /// </summary>
        JobInformation JobInformation
        {
            get;
        }

        /// <summary>
        /// Full name of the job.
        /// </summary>
        string Key
        {
            get;
        }

        /// <summary>
        /// Job's instance id.
        /// </summary>
        string InstanceId
        {
            get;
        }
    }
}
