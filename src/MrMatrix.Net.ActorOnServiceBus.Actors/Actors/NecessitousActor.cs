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
    [Actor("actor/necessitous")]
    public class NecessitousActor
    {
        private readonly IActorsNetwork<NecessitousSaga> _actorsNetwork;
        private readonly ILogger<NecessitousActor> _logger;

        public NecessitousActor(IActorsNetwork<NecessitousSaga> actorsNetwork, ILogger<NecessitousActor> logger)
        {
            _actorsNetwork = actorsNetwork;
            _logger = logger;
        }

        public Task Handle(NecessityDto necessity, CancellationToken cancellationToken)
        {
            _actorsNetwork.Saga.NecessitousId = necessity.NecessitouId;
            _logger.LogInformation("Registered necessity");
            if (_actorsNetwork.Saga.Balance.ContainsKey(necessity.Key))
            {
                _actorsNetwork.Saga.Balance[necessity.Key].Necessity += necessity.Quantity;
            }
            else
            {
                _actorsNetwork.Saga.Balance[necessity.Key] = new NeedBalance()
                {
                    Donation = 0,
                    Necessity = necessity.Quantity
                };

            }

            _actorsNetwork.ReplyToRequester(new BalanceDto()
            {
                Key = necessity.Key,
                Entered = _actorsNetwork.Saga.Balance[necessity.Key].Necessity,
                Balanced = _actorsNetwork.Saga.Balance[necessity.Key].Donation
            });

            _actorsNetwork.SendMessageTo<NeedBalancerActor, NecessityDto>(necessity.Key, necessity);

            return Task.CompletedTask;
        }

        public Task Handle(BalanceQueryDto query, CancellationToken cancellationToken)
        {
            _actorsNetwork.Saga.NecessitousId = query.PersonId;
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
                _actorsNetwork.Saga.Balance[balancedNeed.Key].Donation += balancedNeed.Quantity;
            }

            return Task.CompletedTask;
        }
    }
}