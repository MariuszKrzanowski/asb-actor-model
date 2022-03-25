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
public class DonorController : ControllerBase
{
    private readonly ILogger<DonorController> _logger;
    private readonly IActorsNetworkExternalClient _actorsMeshClient;

    public DonorController(ILogger<DonorController> logger, IActorsNetworkExternalClient actorsMeshClient)
    {
        _logger = logger;
        _actorsMeshClient = actorsMeshClient;
    }

    [HttpPost("donorId/{donorId}/registerDonation")]
    public async Task<IActionResult> Register([FromRoute] string donorId, [FromBody] Donation donation, CancellationToken cancellationToken)
    {
        var result = await _actorsMeshClient.SendMessage(new DonationDto()
        {
            DonorId = donorId,
            Key = donation.Key,
            Quantity = donation.Quantity
        }).ToActorAndWait<DonorActor>(donorId, cancellationToken);

        return Ok(result);
    }

    [HttpGet("donorId/{donorId}/balance")]
    public async Task<IActionResult> Balance([FromRoute] string donorId, CancellationToken cancellationToken)
    {
        var result = await _actorsMeshClient.SendMessage(new BalanceQueryDto()
        {
            PersonId = donorId,
        }).ToActorAndWait<DonorActor>(donorId, cancellationToken);
        
        return Ok(result);
    }
}