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

        public override JobInformation JobInformation => new JobInformation()
        {
            SystemName = nameof(SampleJob2),
            DisplayName = "Sample Job 2",
            Description = "This is a sample job too and writes to console log."
        };

        public SampleJob2(ILogger<SampleJob2> logger, ILogger<JobBase> jobBsaeLogger)
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
