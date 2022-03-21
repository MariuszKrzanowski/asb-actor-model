using MrMatrix.Net.ActorOnServiceBus.Conventions;
using System;
using System.Collections.Generic;

namespace MrMatrix.Net.ActorOnServiceBus.Messages.Transforms
{
    internal class MessageSerializationHelper : IMessageSerializationHelper
    {
        readonly Dictionary<string, Type> _labelToType = new Dictionary<string, Type>();
        readonly Dictionary<Type, string> _typeToLabel = new Dictionary<Type, string>();
        public MessageSerializationHelper(IEnumerable<IMessageDescriptor> messageDescriptors)
        {
            foreach (var messageDescriptor in messageDescriptors)
            {
                _labelToType[messageDescriptor.MessageLabel] = messageDescriptor.MessageType;
                _typeToLabel[messageDescriptor.MessageType] = messageDescriptor.MessageLabel;
            }
        }

        public Type ResolveType(string label) => _labelToType[label];
        public string ResolveLabel(Type type) => _typeToLabel[type];
        public string Serialize<TMessage>(TMessage message)
        {
            return JsonHelper.SerializeObject(message);
        }

        public object Deserialize(string label, string message)
        {
            return JsonHelper.DeserializeObject(_labelToType[label], message);
        }
    }
}