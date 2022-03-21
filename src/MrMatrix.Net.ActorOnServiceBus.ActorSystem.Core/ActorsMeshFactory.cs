using Azure.Messaging.ServiceBus;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core
{
    public class ActorsNetwork<TSaga> : IActorsNetwork<TSaga> where TSaga : class, new()
    {
        private readonly SessionScope _sessionScope;
        private readonly IServiceBusTopicsFactory _serviceBusTopicsFactory;
        private readonly IMessageSerializationHelper _messageSerializationHelper;
        private TSaga? _saga;

        public ActorsNetwork(SessionScope sessionScope, IServiceBusTopicsFactory serviceBusTopicsFactory,
            IMessageSerializationHelper messageSerializationHelper)
        {
            _sessionScope = sessionScope;
            _serviceBusTopicsFactory = serviceBusTopicsFactory;
            _messageSerializationHelper = messageSerializationHelper;
        }

        public void SendMessageTo<TActor, TMessage>(string actorId, TMessage message)
        {
            var msg = new ServiceBusMessage()
            {
                Subject = _messageSerializationHelper.ResolveLabel(message.GetType()),
                Body = new BinaryData(_messageSerializationHelper.Serialize(message)),
                ContentType = "applciation/json",
                SessionId = actorId,
                PartitionKey = actorId,
                CorrelationId = Guid.NewGuid().ToString("N"),
                MessageId = Guid.NewGuid().ToString("N") //,
                //TimeToLive = originatingMessage.TimeToLive
            };
            _sessionScope.PlaceInOutbox(_serviceBusTopicsFactory.FindTopic(typeof(TActor)), msg);
        }

        public void ReplyToRequester<TMessage>(TMessage message)
        {
            var originatingMessage = _sessionScope.ProccessedMessageContext.Message;

            var msg = new ServiceBusMessage()
            {
                Subject = _messageSerializationHelper.ResolveLabel(message.GetType()),
                Body = new BinaryData(_messageSerializationHelper.Serialize(message)),
                ContentType = "applciation/json",
                SessionId = originatingMessage.ReplyToSessionId,
                PartitionKey = originatingMessage.ReplyToSessionId,
                CorrelationId = originatingMessage.MessageId,
                MessageId = Guid.NewGuid().ToString("N"),
                TimeToLive = originatingMessage.TimeToLive
            };
            _sessionScope.PlaceInOutbox(originatingMessage.ReplyTo, msg);
        }

        public TSaga Saga
        {
            get
            {
                if (_saga == null)
                {
                    _saga = new TSaga();
                }

                return _saga;
            }
        }
    }
}