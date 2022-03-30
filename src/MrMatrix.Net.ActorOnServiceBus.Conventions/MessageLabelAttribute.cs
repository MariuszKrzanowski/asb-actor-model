using System;

namespace MrMatrix.Net.ActorOnServiceBus.Conventions
{

    /// <summary>
    /// Marks serialized message with specific label in Azure Service Bus.
    /// It is used to help with serialize/deserialize process.
    /// </summary>
    public class MessageLabelAttribute : Attribute
    {
        public string Label { get; }

        public MessageLabelAttribute(string name)
        {
            Label = name;
        }
    }
}