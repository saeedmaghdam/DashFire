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

        public override JobInformation JobInformation => new JobInformation()
        {
            SystemName = nameof(SampleJob1),
            DisplayName = "Sample Job 1",
            Description = "This is a sample job to test the package functionality."
        };

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
            do
            {
                _logger.LogError("SampleJob1: Processing ...");

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            } while (!cancellationToken.IsCancellationRequested);
        }
    }
}
