using MrMatrix.Net.ActorOnServiceBus.Conventions;
using System;
using System.Linq;

namespace MrMatrix.Net.ActorOnServiceBus.Messages.Transforms
{
    internal class MessageDescriptor<TMessage> : IMessageDescriptor
    {
        public MessageDescriptor()
        {
            MessageType = typeof(TMessage);
            MessageLabel = typeof(TMessage).GetCustomAttributes(false).Where(a => a.GetType() == typeof(MessageLabelAttribute)).Cast<MessageLabelAttribute>()
                .First().Label;
        }

        public Type MessageType { get; }
        public string MessageLabel { get; }
    }
}