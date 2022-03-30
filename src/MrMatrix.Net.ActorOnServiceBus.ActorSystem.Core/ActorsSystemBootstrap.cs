using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core;

public class SessionScope
{
    public class MessageToBeSent
    {
        public ServiceBusMessage Message { get; set; }
        public string Topic { get; set; }
    }

    private List<MessageToBeSent> _outbox = new List<MessageToBeSent>();

    public IEnumerable<MessageToBeSent> ReadOutbox()
    {
        return _outbox;
    }

    public string? SessionId { get; set; }

    public ProcessSessionMessageEventArgs? ProccessedMessageContext { get; set; }

    public void PlaceInOutbox(string topic, ServiceBusMessage msg)
    {
        _outbox.Add(new MessageToBeSent() { Topic = topic, Message = msg });
    }

    public void ClearOutbox()
    {
        _outbox.Clear();
    }
}

public static class ActorsSystemBootstrap
{


    public static IServiceCollection ConfigureActorSystem(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusConnectionString = configuration["ActorSystem:ServiceBusConnectionString"];
        services.AddSingleton<ServiceBusClient>((s) => new ServiceBusClient(serviceBusConnectionString));

        services.AddSingleton<IServiceBusTopicsFactory, ServiceBusTopicsFactory>();
        services.AddSingleton<IActorsNetworkExternalClient, ActorsMeshClient>();
        services.AddSingleton<IActorSystem, ActorSystem>();
        services.AddScoped<SessionScope>();
        return services;
    }

    public static IServiceCollection RegisterActor<TActor, TSaga>(this IServiceCollection services, MessageDirection direction)
        where TActor : class
        where TSaga : class, new()
    {
        services.AddScoped<TActor>();
        services.AddScoped<IActorsNetwork<TSaga>, ActorsNetwork<TSaga>>();
        services.AddTransient<IActorMetadata>((s) => new ActorMetadata<TActor>(direction));
        return services;
    }
}