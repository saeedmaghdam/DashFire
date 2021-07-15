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
        /// Starts the job.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a task.</returns>
        Task StartAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Stops the job.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a task.</returns>
        Task StopAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Shutdowns the job.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a task.</returns>
        Task ShutdownAsync(CancellationToken cancellationToken);
    }
}
