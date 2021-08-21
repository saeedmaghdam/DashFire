using System.Threading;
using System.Threading.Tasks;

namespace DashFire.Framework
{
    /// <summary>
    /// DashLogger which handles DashFire level logs and send logs to Dashboard.
    /// </summary>
    public interface IDashLogger
    {
        /// <summary>
        /// Logs DashFire level logs and send to the Dashboard.
        /// </summary>
        /// <param name="key">Job's Key</param>
        /// <param name="instanceId">Job's instance id</param>
        /// <param name="message">Log message content</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task LogAsync(string key, string instanceId, string message, CancellationToken cancellationToken);

        /// <summary>
        /// Register a job as registered.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="instanceId"></param>
        void Register(string key, string instanceId);
    }
}
