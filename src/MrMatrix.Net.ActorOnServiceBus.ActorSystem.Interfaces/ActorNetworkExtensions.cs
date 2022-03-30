namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces
{
    /// <summary>
    /// This class helps build more fluent language to sent messages.
    /// E.g.  `.SendMessage(message).ToActor<TActor>(actorId) `
    /// </summary>
    public static class ActorNetworkExtensions
    {
        public class SendMessageContext<TSaga, TMessage>
        {
            private readonly IActorsNetwork<TSaga> _actorsNetwork;
            private readonly TMessage _message;

            internal SendMessageContext(IActorsNetwork<TSaga> actorsNetwork, TMessage message)
            {
                _actorsNetwork = actorsNetwork;
                _message = message;
            }

            public void ToActor<TActor>(string actorId)
            {
                _actorsNetwork.SendMessageTo<TActor, TMessage>(actorId, _message);
            }
        }

        public static SendMessageContext<TSaga, TMessage> SendMessage<TSaga, TMessage>(this IActorsNetwork<TSaga> actorsNetwork, TMessage message) where TSaga : class, new()
        {
            return new SendMessageContext<TSaga, TMessage>(actorsNetwork, message);
        }
    }
}