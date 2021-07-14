﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DashFire
{
    public abstract class JobBase : IJob
    {
        private readonly ILogger<JobBase> _logger;

        public abstract JobInformation JobInformation
        {
            get;
        }

        protected JobBase()
        {
            _logger = (ILogger<JobBase>)JobContext.Instance.ServiceProvider.GetService(typeof(ILogger<JobBase>));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{JobInformation.Key} Started.");

            await StartInternallyAsync(cancellationToken);
        }

        protected abstract internal Task StartInternallyAsync(CancellationToken cancellationToken);

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{JobInformation.Key} Stoped!");

            await StopInternallyAsync(cancellationToken);
        }

        protected virtual Task StopInternallyAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task ShutdownAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{JobInformation.Key} Shutdown!");

            await ShutdownInternallyAsync(cancellationToken);
        }

        protected virtual Task ShutdownInternallyAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
