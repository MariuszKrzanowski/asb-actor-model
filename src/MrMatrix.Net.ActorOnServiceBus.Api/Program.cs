using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrMatrix.Net.ActorOnServiceBus.Actors.Actors;
using MrMatrix.Net.ActorOnServiceBus.Messages;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using MrMatrix.Net.ActorOnServiceBus.Actors.Sagas;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;

namespace MrMatrix.Net.ActorOnServiceBus.Api;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.Title = "API";
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.ConfigureActorSystem(builder.Configuration);
        builder.Services.ConfigureAllMessages();
        


        builder.Services
            .RegisterActor<DonorActor, DonationSaga>(MessageDirection.Out)
            .RegisterActor<NecessitousActor, NecessitousSaga>(MessageDirection.Out)
            ;

        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            // based on blog article: https://andrewlock.net/adding-cache-control-headers-to-static-files-in-asp-net-core/
            app.UseStaticFiles(new StaticFileOptions()
                {
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] = "no-cache"; // To simplify DEV 
                    }
                }
            );
        }
        else
        {
            app.UseStaticFiles();
        }

        app.MapControllers();

        
        CancellationToken cancellationToken = default;
        try
        {
            await app.StartAsync(cancellationToken);
            IProcessorsCollection actorsMeshWaitingClient = (IProcessorsCollection)(app.Services.GetService<IActorsNetworkExternalClient>());
            await actorsMeshWaitingClient.StartAsync(cancellationToken);

            IActorSystem actorSystem = (IActorSystem)app.Services.GetService<IActorSystem>();
            actorSystem.StartAsync(cancellationToken);
            await app.WaitForShutdownAsync(cancellationToken);
            await actorsMeshWaitingClient.StopAsync(cancellationToken);
            actorSystem.StopAsync(cancellationToken);
        }
        finally
        {
            await app.DisposeAsync();
        }
    }
}