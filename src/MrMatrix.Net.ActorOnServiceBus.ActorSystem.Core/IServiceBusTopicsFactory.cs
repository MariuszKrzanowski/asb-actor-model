using Azure.Messaging.ServiceBus;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core
{

    public interface IServiceBusTopicsFactory
    {
        Task SendMessageAsync(string topic, ServiceBusMessage message, CancellationToken cancellationToken);
        Task SendMessageAsync(Type actorType, ServiceBusMessage message, CancellationToken cancellationToken);
        string FindTopic(Type type);
    }
}