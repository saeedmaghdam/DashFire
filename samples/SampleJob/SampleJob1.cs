using System;
using System.Threading;
using System.Threading.Tasks;
using DashFire;
using Microsoft.Extensions.Logging;

namespace SampleJob
{
    public class SampleJob1 : JobBase
    {
        private readonly ILogger<SampleJob1> _logger;

        public SampleJob1(ILogger<SampleJob1> logger, ILogger<JobBase> jobBaseLogger)
        {
            _logger = logger;
        }

        protected override async Task StartInternallyAsync(CancellationToken cancellationToken)
        {
            do
            {
                _logger.LogError("SampleJob1: Processing ...");

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            } while (!cancellationToken.IsCancellationRequested);
        }
    }
}
