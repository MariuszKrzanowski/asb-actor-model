using System;

namespace MrMatrix.Net.ActorOnServiceBus.Messages.Transforms
{
    internal interface IMessageDescriptor
    {
        public Type MessageType { get; }
        public string MessageLabel { get; }
    }
}