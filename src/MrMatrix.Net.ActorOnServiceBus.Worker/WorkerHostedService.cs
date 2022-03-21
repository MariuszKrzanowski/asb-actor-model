using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;

namespace MrMatrix.Net.ActorOnServiceBus.Worker;

public class WorkerHostedService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IActorSystem _actorSystem;

    public WorkerHostedService(
        ILogger<WorkerHostedService> logger,
        IHostApplicationLifetime appLifetime,
        IActorSystem actorSystem
    )
    {
        _logger = logger;
        _actorSystem = actorSystem;

        appLifetime.ApplicationStarted.Register(OnStarted);
        appLifetime.ApplicationStopping.Register(OnStopping);
        appLifetime.ApplicationStopped.Register(OnStopped);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("1. StartAsync has been called.");
        await _actorSystem.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _actorSystem.StopAsync(cancellationToken);
        _logger.LogInformation("4. StopAsync has been called.");
    }

    private void OnStarted()
    {
        _logger.LogInformation("2. OnStarted has been called.");
    }

    private void OnStopping()
    {
        _logger.LogInformation("3. OnStopping has been called.");
    }

    private void OnStopped()
    {
        _logger.LogInformation("5. OnStopped has been called.");
    }
}