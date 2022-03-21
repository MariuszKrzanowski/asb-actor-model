using System.Threading;
using System.Threading.Tasks;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces
{
    public interface IActorMessageHandler
    {
        Task Handle(object actor, object message, CancellationToken cancellationToken);

    }
}