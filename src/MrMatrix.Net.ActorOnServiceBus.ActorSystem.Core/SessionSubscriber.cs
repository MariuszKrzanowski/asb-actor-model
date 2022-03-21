using System.Collections.Concurrent;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core;

public class SessionSubscriber
{
    private readonly ServiceBusSessionProcessor _processor;
    private readonly IServiceBusTopicsFactory _serviceBusTopicsFactory;
    private readonly IMessageSerializationHelper _messageSerializationHelper;
    private readonly IActorMessageHandler _actorMessageHandler;
    private readonly IServiceScopeFactory _serviceServiceScopeFactory;
    private readonly Type _actorType;

    private readonly ConcurrentDictionary<string, SessionConsumer> _subscribedActors;

    // SessionId => Saga
    // SessionId => Actor
    // Label to message handler

    public SessionSubscriber(ServiceBusSessionProcessor processor, IServiceBusTopicsFactory serviceBusTopicsFactory, IMessageSerializationHelper messageSerializationHelper, IActorMessageHandler actorMessageHandler, IServiceScopeFactory serviceServiceScopeFactory, Type actorType)
    {
        _processor = processor;
        _serviceBusTopicsFactory = serviceBusTopicsFactory;
        _messageSerializationHelper = messageSerializationHelper;
        _actorMessageHandler = actorMessageHandler;
        _serviceServiceScopeFactory = serviceServiceScopeFactory;
        _actorType = actorType;
        _subscribedActors = new ConcurrentDictionary<string, SessionConsumer>();

        _processor.SessionInitializingAsync += InitSaga;
        _processor.SessionClosingAsync += ReleaseSaga;
        _processor.ProcessErrorAsync += ReportError;
        _processor.ProcessMessageAsync += HandleMessage;
    }

    private async Task HandleMessage(ProcessSessionMessageEventArgs arg)
    {
        await _subscribedActors[arg.SessionId].Handle(arg);
    }

    private Task ReportError(ProcessErrorEventArgs arg)
    {
        return Task.CompletedTask;
    }

    private Task ReleaseSaga(ProcessSessionEventArgs arg)
    {
        if (_subscribedActors.TryRemove(arg.SessionId, out var session))
        {
            session.Dispose();
        }
        return Task.CompletedTask;
    }

    private Task InitSaga(ProcessSessionEventArgs arg)
    {
        _subscribedActors.TryAdd(arg.SessionId, new SessionConsumer(arg.SessionId, _serviceServiceScopeFactory, _actorMessageHandler, _messageSerializationHelper, _serviceBusTopicsFactory, _actorType));
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _processor.StartProcessingAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
    }
}