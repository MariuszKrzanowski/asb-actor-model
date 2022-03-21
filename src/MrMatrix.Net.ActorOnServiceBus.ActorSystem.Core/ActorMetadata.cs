using System.Reflection;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core;

public class ActorMetadata<TActor> : IActorMetadata
{
    public Type ActorType { get; }
    public string TopicUri { get; }
    public IActorMessageHandler CreateActorMessageHandler()
    {
        return new ActorMessageHandler(ActorType);
    }

    private class ActorMessageHandler : IActorMessageHandler
    {
        private readonly Dictionary<Type, MessageHandler> _handlers;

        public ActorMessageHandler(Type actorType)
        {
            _handlers = actorType.GetMethods()
                 .Where(MethodReturnsTask)
                 .Where(MethodHasAccessToObjectInstance)
                 .Where(MethodHasDto)
                 .Select(mi => new MessageHandler(mi))
                 .ToDictionary(s => s.MessageType, s => s);
        }

        public Task? Handle(object actor, object message, CancellationToken cancellationToken)
        {
            return _handlers[message.GetType()].Handle((TActor)actor, message, cancellationToken);
        }
    }

    public ActorMetadata(MessageDirection direction)
    {
        Direction = direction;
        ActorType = typeof(TActor);
        TopicUri = ResolveTopic();
    }

    private string ResolveTopic()
    {
        var metadata = ActorType.GetCustomAttributes(false)
            .Where(t => t.GetType() == typeof(ActorAttribute))
            .Cast<ActorAttribute>()
            .FirstOrDefault();

        if (metadata is null)
        {
            throw new Exception("TODO");
        }

        return metadata.HierarchyUri;
    }

    public class MessageHandler
    {
        private readonly MethodInfo _methodInfo;

        public MessageHandler(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            MessageType = methodInfo.GetParameters()[0].ParameterType;
        }

        public Type MessageType { get; }

        public Task? Handle(TActor actor, object message, CancellationToken cancellationToken)
        {
            return (Task?)(_methodInfo.Invoke(actor, new[] { message, cancellationToken }));
        }
    }



    private static bool MethodHasDto(MethodInfo mi)
    {
        return mi.GetParameters()?.Length == 2
               && mi.GetParameters()[0].ParameterType.GetCustomAttributes(false)
                   .Any(t => t.GetType() == typeof(MessageLabelAttribute)
                   && mi.GetParameters()[1].ParameterType == typeof(CancellationToken)
                   );
    }

    private static bool MethodHasAccessToObjectInstance(MethodInfo mi)
    {
        return 0 != (mi.CallingConvention & CallingConventions.HasThis);
    }

    private static bool MethodReturnsTask(MethodInfo mi)
    {
        return mi.ReturnType == typeof(Task);
    }

    public MessageDirection Direction { get; }
}