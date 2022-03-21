using Azure.Messaging.ServiceBus;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core;

public class ServiceBusTopicsFactory : IServiceBusTopicsFactory
{
    private readonly ServiceBusClient _serviceBusClient;
    private Dictionary<string, ServiceBusSender> _sendersByName;
    private Dictionary<Type, ServiceBusSender> _sendersByType;
    private Dictionary<Type, string> _senderToTopic;

    public ServiceBusTopicsFactory(ServiceBusClient serviceBusClient, IEnumerable<IActorMetadata> actorsMetadata)
    {
        _serviceBusClient = serviceBusClient;
        _sendersByName = new Dictionary<string, ServiceBusSender>();
        _sendersByType = new Dictionary<Type, ServiceBusSender>();
        _senderToTopic = new Dictionary<Type, string>();
        foreach (var actorMetadata in actorsMetadata)
        {
            RegisterActor(actorMetadata);
        }

        var sender = _serviceBusClient.CreateSender("web-client");
        _sendersByName["web-client"] = sender;
    }

    private void RegisterActor(IActorMetadata actorMetadata)
    {
        var topic = actorMetadata.TopicUri;
        if ((actorMetadata.Direction & MessageDirection.Out) != 0)
        {
            var sender = _serviceBusClient.CreateSender(topic);
            _sendersByName[topic] = sender;
            _sendersByType[actorMetadata.ActorType] = sender;
            _senderToTopic[actorMetadata.ActorType] = topic;
        }
    }

    public Task SendMessageAsync(string topic, ServiceBusMessage message, CancellationToken cancellationToken)
    {
        return _sendersByName[topic].SendMessageAsync(message, cancellationToken);
    }

    public Task SendMessageAsync(Type type, ServiceBusMessage message, CancellationToken cancellationToken)
    {
        return _sendersByType[type].SendMessageAsync(message, cancellationToken);
    }

    public string FindTopic(Type type) => _senderToTopic[type];
}