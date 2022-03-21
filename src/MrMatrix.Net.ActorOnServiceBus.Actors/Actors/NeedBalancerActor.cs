using Microsoft.Extensions.Logging;
using MrMatrix.Net.ActorOnServiceBus.Messages.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MrMatrix.Net.ActorOnServiceBus.Actors.Sagas;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.Actors.Actors
{
    [Actor("actor/need-balancer")]
    public class NeedBalancerActor
    {
        private readonly IActorsNetwork<NeedsBalancerSaga> _actorsNetwork;
        private readonly ILogger<NeedBalancerActor> _logger;

        public NeedBalancerActor(IActorsNetwork<NeedsBalancerSaga> actorsNetwork, ILogger<NeedBalancerActor> logger)
        {
            _actorsNetwork = actorsNetwork;
            _logger = logger;
        }

        public Task Handle(DonationDto donation, CancellationToken cancellationToken)
        {
            _actorsNetwork.Saga.Key = donation.Key;
            _logger.LogInformation("Registered necessity");

            _actorsNetwork.Saga.Donations.Add(new RegisteredNeed()
            {
                PersonId = donation.DonorId,
                Quantity = donation.Quantity
            });

            BalanceNeedsAndNotifyParticipants();

            return Task.CompletedTask;
        }

        public Task Handle(NecessityDto necessity, CancellationToken cancellationToken)
        {
            _actorsNetwork.Saga.Key = necessity.Key;
            _logger.LogInformation("Registered necessity");

            _actorsNetwork.Saga.Necessities.Add(new RegisteredNeed()
            {
                PersonId = necessity.NecessitouId,
                Quantity = necessity.Quantity
            });

            BalanceNeedsAndNotifyParticipants();

            return Task.CompletedTask;
        }

        private void BalanceNeedsAndNotifyParticipants()
        {
            foreach (var balancedNeed in BalanceNeeds(_actorsNetwork.Saga))
            {
                _actorsNetwork.SendMessageTo<NecessitousActor, BalancedNeed2Dto>(balancedNeed.NecessitiousId, new BalancedNeed2Dto { Key = _actorsNetwork.Saga.Key, Quantity = balancedNeed.Donations.Sum(d => d.Quantity) });

                foreach (var donations in balancedNeed.Donations)
                {
                    _actorsNetwork.SendMessageTo<DonorActor, BalancedNeed2Dto>(donations.PersonId, new BalancedNeed2Dto { Key = _actorsNetwork.Saga.Key, Quantity = donations.Quantity });
                }
            }
        }

        private static IEnumerable<BalancedNeed> BalanceNeeds(NeedsBalancerSaga needsBalancer)
        {
            var donations = new Queue<RegisteredNeed>(needsBalancer.Donations.Where(d => d.Quantity > 0).ToList());
            var necessities = new Queue<RegisteredNeed>(needsBalancer.Necessities.Where(d => d.Quantity > 0).ToList());


            while (necessities.TryPeek(out var necessity))
            {
                var left = necessity.Quantity;

                var balancedNeed = new BalancedNeed()
                {
                    Quantity = necessity.Quantity,
                    NecessitiousId = necessity.PersonId,
                    Donations = new List<MatchedDonation>()
                };

                while (donations.TryPeek(out var donation))
                {
                    if (left < donation.Quantity)
                    {
                        balancedNeed.Donations.Add(new MatchedDonation()
                        {
                            Quantity = left,
                            PersonId = donation.PersonId
                        });

                        donation.Quantity -= left;
                        left = 0;
                        break;
                    }

                    donations.Dequeue();
                    balancedNeed.Donations.Add(new MatchedDonation()
                    {
                        Quantity = donation.Quantity,
                        PersonId = donation.PersonId
                    });
                    left -= donation.Quantity;
                }

                if (left == 0)
                {
                    necessities.Dequeue();
                }

                if (balancedNeed.Donations.Count > 0)
                {
                    necessity.Quantity -= balancedNeed.Donations.Sum(d => d.Quantity);
                    yield return balancedNeed;
                }

                if (donations.Count == 0)
                {
                    break;
                }
            }

            needsBalancer.Donations = donations.ToList();
            needsBalancer.Necessities = necessities.ToList();
        }
    }
}