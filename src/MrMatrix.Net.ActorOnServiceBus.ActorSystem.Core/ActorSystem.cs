using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core;
public class ActorSystem : IActorSystem
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IServiceBusTopicsFactory _serviceBusTopicsFactory;
    private readonly IMessageSerializationHelper _messageSerializationHelper;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private List<SessionSubscriber> _subscribers;

    public ActorSystem(ServiceBusClient serviceBusClient, IEnumerable<IActorMetadata> actorsMetadata, IServiceBusTopicsFactory serviceBusTopicsFactory, IMessageSerializationHelper messageSerializationHelper, IServiceScopeFactory serviceScopeFactory)
    {
        _serviceBusClient = serviceBusClient;
        _serviceBusTopicsFactory = serviceBusTopicsFactory;
        _messageSerializationHelper = messageSerializationHelper;
        _serviceScopeFactory = serviceScopeFactory;

        _subscribers = new List<SessionSubscriber>();
        foreach (var actorMetadata in actorsMetadata)
        {
            RegisterActor(actorMetadata);
        }
    }

    private void RegisterActor(IActorMetadata actorMetadata)
    {
        var topic = actorMetadata.TopicUri;

        if ((actorMetadata.Direction & MessageDirection.In) != 0)
        {
            var processor = _serviceBusClient.CreateSessionProcessor(topic, "inbox");
            _subscribers.Add(new SessionSubscriber(processor, _serviceBusTopicsFactory, _messageSerializationHelper, actorMetadata.CreateActorMessageHandler(), _serviceScopeFactory, actorMetadata.ActorType));
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var subscriber in _subscribers)
        {
            await subscriber.StartAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var subscriber in _subscribers)
        {
            await subscriber.StopAsync(cancellationToken);
        }
    }
}