using System.Threading;
using System.Threading.Tasks;
using DashFire;
using Microsoft.Extensions.Logging;

namespace SampleJob
{
    public class SampleJob2 : JobBase
    {
        private readonly ILogger<SampleJob2> _logger;

        public override JobInformation JobInformation => JobInformationBuilder.CreateInstance()
            .SetSystemName(nameof(SampleJob2))
            .SetDisplayName("Sample Job 2")
            .SetDescription("This is a sample job too and writes to console log.")
            .SetCronSchedules(new[] { "*/2 * * * *", "*/5 * * * *" })
            .Build();

        public SampleJob2(ILogger<SampleJob2> logger, ILogger<JobBase> jobBsaeLogger)
        {
            _logger = logger;
        }

        protected override Task StartInternallyAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Service2: DashFire fires me due to cron schedules! DashFire schedules me and I prommise I'll back soon!");

            return Task.CompletedTask;
        }
    }
}
