using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core;

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