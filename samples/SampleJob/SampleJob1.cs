using System;
using System.Threading;
using System.Threading.Tasks;
using DashFire.Attributes;
using Microsoft.Extensions.Logging;

namespace DashFire.Service.Sample
{
    public class SampleJob1 : Job
    {
        private readonly ILogger<SampleJob1> _logger;

        public override JobInformation JobInformation => JobInformationBuilder.CreateInstance()
            .SetSystemName(nameof(SampleJob1))
            .SetDisplayName("Sample Job 1")
            .SetDescription("This is a sample job to test the package functionality.")
            .RegistrationRequired()
            .JobInstanceIdRequired()
            .Build();

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

        public SampleJob1(ILogger<SampleJob1> logger, ILogger<Job> jobBaseLogger)
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
