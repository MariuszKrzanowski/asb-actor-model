using System;

namespace MrMatrix.Net.ActorOnServiceBus.Conventions
{
    public interface IMessageSerializationHelper
    {
        Type ResolveType(string label);
        string ResolveLabel(Type type);

        string Serialize<TMessage>(TMessage message);
        object Deserialize(string label, string message);
    }
}