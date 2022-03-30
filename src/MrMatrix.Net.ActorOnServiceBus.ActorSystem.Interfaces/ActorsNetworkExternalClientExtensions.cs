using System.Threading;
using System.Threading.Tasks;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces
{
    /// <summary>
    /// This class helps build more fluent language to sent messages.
    /// E.g.  `.SendMessage(message).ToActorAndWait<TActor>(actorId,cancellationToken) `
    /// </summary>
    public static class ActorsNetworkExternalClientExtensions
    {
        public class SendMessageContext<TMessage>
        {
            private readonly IActorsNetworkExternalClient _actorsNetwork;
            private readonly TMessage _message;

            internal SendMessageContext(IActorsNetworkExternalClient actorsNetwork, TMessage message)
            {
                _actorsNetwork = actorsNetwork;
                _message = message;
            }


            public Task<object> ToActorAndWait<TActor>(string actorId, CancellationToken cancellationToken)
            {
                return _actorsNetwork.SendMessageToAndWait<TActor, TMessage>(actorId, _message, cancellationToken);
            }
        }

        public static SendMessageContext<TMessage> SendMessage<TMessage>(this IActorsNetworkExternalClient actorsNetwork, TMessage message)
        {
            return new SendMessageContext<TMessage>(actorsNetwork, message);
        }
    }
}