using Microsoft.Extensions.Logging;
using MrMatrix.Net.ActorOnServiceBus.Actors.Sagas;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using MrMatrix.Net.ActorOnServiceBus.Conventions;
using MrMatrix.Net.ActorOnServiceBus.Messages.Dtos;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            EnsureBalanceExists(donation.Key);

            _actorsNetwork.Saga.Balance[donation.Key].Donation += donation.Quantity;

            _actorsNetwork.ReplyToRequester(new BalanceDto()
            {
                Key = donation.Key,
                Entered = _actorsNetwork.Saga.Balance[donation.Key].Donation,
                Balanced = _actorsNetwork.Saga.Balance[donation.Key].Necessity
            });

            _actorsNetwork.SendMessage(donation).ToActor<NeedBalancerActor>(donation.Key);

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

        private void EnsureBalanceExists(string donationKey)
        {
            if (!_actorsNetwork.Saga.Balance.ContainsKey(donationKey))
            {
                _actorsNetwork.Saga.Balance[donationKey] = new NeedBalance()
                {
                    Donation = 0,
                    Necessity = 0
                };
            }
        }
    }
}