using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core
{

    public interface IActorMetadata
    {
        Type ActorType { get; }
        MessageDirection Direction { get; }
        string TopicUri { get; }
        IActorMessageHandler CreateActorMessageHandler();
    }
}