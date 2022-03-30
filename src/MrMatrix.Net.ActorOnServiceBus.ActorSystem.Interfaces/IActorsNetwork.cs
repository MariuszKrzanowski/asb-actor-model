namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces
{
    public interface IActorsNetwork<out TSaga>
    {
        TSaga Saga { get; }
        void SendMessageTo<TActor, TMessage>(string actorId, TMessage message);
        void ReplyToRequester<TMessage>(TMessage message);
    }
}