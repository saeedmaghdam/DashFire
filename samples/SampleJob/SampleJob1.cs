using System;
using System.Threading;
using System.Threading.Tasks;
using DashFire;
using DashFire.Attributes;
using Microsoft.Extensions.Logging;

namespace SampleJob
{
    public class SampleJob1 : JobBase
    {
        private readonly ILogger<SampleJob1> _logger;

        public override JobInformation JobInformation => new JobInformation(nameof(SampleJob1), "Sample Job 1", "This is a sample job to test the package functionality.");

        [JobParameter("Start Date", "Start date of calculation")]
        public DateTime StartDate
        {
            get;
            set;
        }

        [JobParameter]
        public int ItemsCount
        {
            get;
            set;
        }

        public SampleJob1(ILogger<SampleJob1> logger, ILogger<JobBase> jobBaseLogger)
        {
            _logger = logger;
        }

        protected override async Task StartInternallyAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SampleJob1: I don't have any schedule and I'll execute til service is on!");

            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        }
    }
}
