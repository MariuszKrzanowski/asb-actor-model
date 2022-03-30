using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MrMatrix.Net.ActorOnServiceBus.Actors.Actors;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using MrMatrix.Net.ActorOnServiceBus.Messages.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace MrMatrix.Net.ActorOnServiceBus.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class NecessitousController : ControllerBase
{
    private readonly ILogger<NecessitousController> _logger;
    private readonly IActorsNetworkExternalClient _actorsMeshClient;

    public NecessitousController(ILogger<NecessitousController> logger, IActorsNetworkExternalClient actorsMeshClient)
    {
        _logger = logger;
        _actorsMeshClient = actorsMeshClient;
    }

    [HttpPost("necessitousId/{necessitousId}/registerNecessity")]
    public async Task<IActionResult> Register([FromRoute] string necessitousId, [FromBody] Necessity necessity, CancellationToken cancellationToken)
    {
        var result = await _actorsMeshClient.SendMessageToAndWait<NecessitousActor, NecessityDto>(necessitousId, new NecessityDto()
        {
            NecessitousId = necessitousId,
            Key = necessity.Key,
            Quantity = necessity.Quantity
        }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("necessitousId/{necessitousId}/balance")]
    public async Task<IActionResult> Balance([FromRoute] string necessitousId, CancellationToken cancellationToken)
    {
        var result = await _actorsMeshClient.SendMessageToAndWait<NecessitousActor, BalanceQueryDto>(necessitousId, new BalanceQueryDto()
        {
            PersonId = necessitousId,
        }, cancellationToken);
        return Ok(result);
    }
}