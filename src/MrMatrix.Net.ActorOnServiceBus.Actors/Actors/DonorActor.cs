using Microsoft.Extensions.Logging;
using MrMatrix.Net.ActorOnServiceBus.Messages.Dtos;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MrMatrix.Net.ActorOnServiceBus.Actors.Sagas;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.Actors.Actors
{
    [Actor("actor/donor")]
    public class DonorActor
    {
        private readonly IActorsNetwork<DonationSaga> _actorsNetwork;
        private readonly ILogger<DonorActor> _logger;

        public DonorActor(IActorsNetwork<DonationSaga> actorsNetwork, ILogger<DonorActor> logger)
        {
            _actorsNetwork = actorsNetwork;
            _logger = logger;
        }

        public Task Handle(DonationDto donation, CancellationToken cancellationToken)
        {
            _actorsNetwork.Saga.DonorId = donation.DonorId;
            _logger.LogInformation("Registered necessity");
            if (_actorsNetwork.Saga.Balance.ContainsKey(donation.Key))
            {
                _actorsNetwork.Saga.Balance[donation.Key].Donation += donation.Quantity;
            }
            else
            {
                _actorsNetwork.Saga.Balance[donation.Key] = new NeedBalance()
                {
                    Donation = donation.Quantity,
                    Necessity = 0
                };
            }

            _actorsNetwork.ReplyToRequester(new BalanceDto()
            {
                Key = donation.Key,
                Entered = _actorsNetwork.Saga.Balance[donation.Key].Donation,
                Balanced = _actorsNetwork.Saga.Balance[donation.Key].Necessity
            });

            _actorsNetwork.SendMessageTo<NeedBalancerActor, DonationDto>(donation.Key, donation);

            return Task.CompletedTask;
        }

        public Task Handle(BalanceQueryDto query, CancellationToken cancellationToken)
        {
            _actorsNetwork.Saga.DonorId = query.PersonId;
            _actorsNetwork.ReplyToRequester(new BalanceResponseDto()
            {
                PersonId = query.PersonId,
                Balance = _actorsNetwork.Saga.Balance.Select(kv =>
                    new BalanceResponseNeed()
                    {
                        Key = kv.Key,
                        Donation = kv.Value.Donation,
                        Necessity = kv.Value.Necessity
                    }).ToList()

            });
            return Task.CompletedTask;
        }

        public Task Handle(BalancedNeed2Dto balancedNeed, CancellationToken cancellationToken)
        {
            if (_actorsNetwork.Saga.Balance.ContainsKey(balancedNeed.Key))
            {
                _actorsNetwork.Saga.Balance[balancedNeed.Key].Necessity += balancedNeed.Quantity;
            }

            return Task.CompletedTask;
        }
    }
}