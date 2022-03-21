using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MrMatrix.Net.ActorOnServiceBus.Actors.Actors;
using MrMatrix.Net.ActorOnServiceBus.Messages.Dtos;
using System.Threading;
using System.Threading.Tasks;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;

namespace MrMatrix.Net.ActorOnServiceBus.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class NecessitousController : ControllerBase
{
    private readonly ILogger<NecessitousController> _logger;
    private readonly IActorSystemExternalClient _actorsMeshClient;

    public NecessitousController(ILogger<NecessitousController> logger, IActorSystemExternalClient actorsMeshClient)
    {
        _logger = logger;
        _actorsMeshClient = actorsMeshClient;
    }

    [HttpPost("necessitiousId/{neccesitiousId}/registerNecessity")]
    public async Task<IActionResult> Register([FromRoute] string neccesitiousId, [FromBody] Neccessity neccessity, CancellationToken cancellationToken)
    {
        var result = await _actorsMeshClient.SendMessageToAndWait<NecessitousActor, NecessityDto>(neccesitiousId, new()
        {
            NecessitouId = neccesitiousId,
            Key = neccessity.Key,
            Quantity = neccessity.Quantity
        }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("necessitiousId/{neccesitiousId}/balance")]
    public async Task<IActionResult> Balance([FromRoute] string neccesitiousId, CancellationToken cancellationToken)
    {
        var result = await _actorsMeshClient.SendMessageToAndWait<NecessitousActor, BalanceQueryDto>(neccesitiousId, new()
        {
            PersonId = neccesitiousId,
        }, cancellationToken);
        return Ok(result);
    }
}