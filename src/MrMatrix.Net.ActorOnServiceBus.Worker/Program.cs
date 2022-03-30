// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrMatrix.Net.ActorOnServiceBus.Actors.Actors;
using MrMatrix.Net.ActorOnServiceBus.Actors.Sagas;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using MrMatrix.Net.ActorOnServiceBus.Messages;
using System;
using System.IO;

namespace MrMatrix.Net.ActorOnServiceBus.Worker;

public static class Program
{


    public static void Main(string[] args)
    {
        Console.Title = "WORKER";
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(configHost =>
            {
                configHost.SetBasePath(Directory.GetCurrentDirectory());
                configHost.AddEnvironmentVariables("NETCORE_");
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.ConfigureActorSystem(hostContext.Configuration);
                services.AddHostedService<WorkerHostedService>();
                services.ConfigureAllMessages();

                services
                    .RegisterActor<DonorActor, DonationSaga>(MessageDirection.Both)
                    .RegisterActor<NecessitousActor, NecessitousSaga>(MessageDirection.Both)
                    .RegisterActor<NeedBalancerActor, NeedsBalancerSaga>(MessageDirection.Both)
                                    ;
            });
}