using System.Threading;
using System.Threading.Tasks;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces
{
    /// <summary>
    /// The helper class. Allows hide all message handlers inside actors.
    /// 
    /// </summary>
    public interface IActorMessageHandler
    {
        Task Handle(object actor, object message, CancellationToken cancellationToken);
    }
}