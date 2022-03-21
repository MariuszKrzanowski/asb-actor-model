using System;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces
{

    public interface IActorMetadata
    {
        Type ActorType { get; }
        MessageDirection Direction { get; }
        string TopicUri { get; }

        IActorMessageHandler CreateActorMessageHandler();
    }
}