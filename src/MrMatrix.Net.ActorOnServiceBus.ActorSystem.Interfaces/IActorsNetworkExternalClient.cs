using System.Threading;
using System.Threading.Tasks;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces
{
    public interface IActorsNetworkExternalClient
    {
        Task<object> SendMessageToAndWait<TActor, TMessage>(string actorId, TMessage message, CancellationToken cancellationToken);
    }
}