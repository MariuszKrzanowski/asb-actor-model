using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core;

public class SessionConsumer : IDisposable
{
    private readonly IActorMessageHandler _actorMessageHandler;
    private readonly IMessageSerializationHelper _messageSerializationHelper;
    private readonly IServiceBusTopicsFactory _serviceBusTopicsFactory;
    private readonly IServiceScope _serviceScope;

    private readonly object _actor;
    private readonly SessionScope _sessionScope;

    public SessionConsumer(string sessionId, IServiceScopeFactory serviceScopeFactory, IActorMessageHandler actorMessageHandler, IMessageSerializationHelper messageSerializationHelper, IServiceBusTopicsFactory serviceBusTopicsFactory, Type actorType)
    {
        _actorMessageHandler = actorMessageHandler;
        _messageSerializationHelper = messageSerializationHelper;
        _serviceBusTopicsFactory = serviceBusTopicsFactory;
        _serviceScope = serviceScopeFactory.CreateScope();

        _sessionScope = (SessionScope?)_serviceScope.ServiceProvider.GetService(typeof(SessionScope)) ?? throw new NotImplementedException("missing session");
        _sessionScope.SessionId = sessionId;
        _actor = _serviceScope.ServiceProvider.GetService(actorType) ?? throw new NotImplementedException("missing actor");
    }

    public async Task Handle(ProcessSessionMessageEventArgs arg)
    {
        try
        {
            _sessionScope.ProccessedMessageContext = arg;
            var message = _messageSerializationHelper.Deserialize(arg.Message.Subject, arg.Message.Body.ToString());
            await _actorMessageHandler.Handle(_actor, message, arg.CancellationToken);

            foreach (var outboxMessages in _sessionScope.ReadOutbox())
            {
                await _serviceBusTopicsFactory.SendMessageAsync(outboxMessages.Topic, outboxMessages.Message, arg.CancellationToken);
            }

            _sessionScope.ClearOutbox();
        }
        finally
        {
            _sessionScope.ProccessedMessageContext = null;
        }
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}