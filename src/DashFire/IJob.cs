using System.Threading;
using System.Threading.Tasks;

namespace DashFire
{
    public interface IJob
    {
        JobInformation JobInformation
        {
            get;
        }

        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);

        Task ShutdownAsync(CancellationToken cancellationToken);
    }
}
