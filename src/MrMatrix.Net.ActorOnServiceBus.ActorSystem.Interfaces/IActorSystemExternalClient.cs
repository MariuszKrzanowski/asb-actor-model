using System.Threading;
using System.Threading.Tasks;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces
{

    public interface IActorSystemExternalClient
    {
        Task<object> SendMessageToAndWait<TActor, TMessage>(string actorId, TMessage message, CancellationToken cancellationToken);
    }
}