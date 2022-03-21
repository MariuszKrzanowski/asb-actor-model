using System.Threading;
using System.Threading.Tasks;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces
{
    public interface IProcessorsCollection
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}