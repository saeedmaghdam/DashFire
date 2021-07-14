using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DashFire
{
    public abstract class JobBase : IJob
    {
        private readonly ILogger<JobBase> _logger;

        protected JobBase(ILogger<JobBase> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Job Started.");

            await StartInternallyAsync(cancellationToken);
        }

        protected abstract internal Task StartInternallyAsync(CancellationToken cancellationToken);

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Job Stoped!");

            await StopInternallyAsync(cancellationToken);
        }

        protected virtual Task StopInternallyAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task ShutdownAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Job Shutdown!");

            await ShutdownInternallyAsync(cancellationToken);
        }

        protected virtual Task ShutdownInternallyAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
