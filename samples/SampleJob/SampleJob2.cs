using System.Threading;
using System.Threading.Tasks;
using DashFire.Framework;
using Microsoft.Extensions.Logging;

namespace DashFire.Service.Sample
{
    public class SampleJob2 : Job
    {
        private readonly ILogger<SampleJob2> _logger;
        private readonly IDashLogger _dashLogger;

        public override JobInformation JobInformation => JobInformationBuilder.CreateInstance()
            .SetSystemName(nameof(SampleJob2))
            .SetDisplayName("Sample Job 2")
            .SetDescription("This is a sample job too and writes to console log.")
            .SetCronSchedules(new[] { "*/2 * * * *", "*/5 * * * *" })
            .RegistrationRequired()
            .Build();

        public SampleJob2(ILogger<SampleJob2> logger, ILogger<Job> jobBsaeLogger, IDashLogger dashLogger)
        {
            _logger = logger;
            _dashLogger = dashLogger;
        }

        protected override Task StartInternallyAsync(CancellationToken cancellationToken)
        {
            _dashLogger.Log(Key, InstanceId, "I'm sending this message from the job!");
            _logger.LogInformation($"Service2: DashFire fires me due to cron schedules! DashFire schedules me and I prommise I'll back soon!");

            return Task.CompletedTask;
        }
    }
}
