using System.Threading;
using System.Threading.Tasks;

namespace DashFire
{
    public interface IJob
    {
        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);

        Task ShutdownAsync(CancellationToken cancellationToken);
    }
}
