using System;
using System.Threading;
using System.Threading.Tasks;
using DashFire;
using Microsoft.Extensions.Logging;

namespace SampleJob
{
    public class SampleJob2 : JobBase
    {
        private readonly ILogger<SampleJob2> _logger;

        public SampleJob2(ILogger<SampleJob2> logger, ILogger<JobBase> jobBsaeLogger) : base(jobBsaeLogger)
        {
            _logger = logger;
        }

        protected override async Task StartInternallyAsync(CancellationToken cancellationToken)
        {
            do
            {
                _logger.LogError("SampleJob2: ++++++++");

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            } while (!cancellationToken.IsCancellationRequested);
        }
    }
}
