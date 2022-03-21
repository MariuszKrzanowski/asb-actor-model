using System;

namespace MrMatrix.Net.ActorOnServiceBus.Conventions
{
    public class MessageLabelAttribute : Attribute
    {
        public string Label { get; }

        public MessageLabelAttribute(string name)
        {
            Label = name;
        }
    }
}