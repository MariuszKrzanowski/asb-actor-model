using Azure.Messaging.ServiceBus;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using MrMatrix.Net.ActorOnServiceBus.Conventions;
using System.Collections.Concurrent;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core;

public class ActorsMeshClient : IActorsNetworkExternalClient, IProcessorsCollection
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IMessageSerializationHelper _messageSerializationHelper;
    private readonly IServiceBusTopicsFactory _topicsFactory;
    private readonly string _replyToSessionId;
    private const string MeshClientTopic = "web-client";

    private readonly ConcurrentDictionary<string, TaskCompletionSource<object>> _returns = new ConcurrentDictionary<string, TaskCompletionSource<object>>();
    private readonly ServiceBusSessionProcessor _processor;

    public ActorsMeshClient(ServiceBusClient serviceBusClient,
        IMessageSerializationHelper messageSerializationHelper,
        IServiceBusTopicsFactory topicsFactory)
    {
        _replyToSessionId = Guid.NewGuid().ToString("N");
        _serviceBusClient = serviceBusClient;
        _messageSerializationHelper = messageSerializationHelper;
        _topicsFactory = topicsFactory;

        _processor = _serviceBusClient.CreateSessionProcessor(MeshClientTopic, "inbox", new ServiceBusSessionProcessorOptions()
        {
            SessionIds = { _replyToSessionId },
            AutoCompleteMessages = false
        });
        _processor.ProcessMessageAsync += HandleReplyMessage;
        _processor.ProcessErrorAsync += HandleErrors;
    }

    private Task HandleErrors(ProcessErrorEventArgs arg)
    {
        return Task.CompletedTask;
    }

    private Task HandleReplyMessage(ProcessSessionMessageEventArgs arg)
    {
        if (_returns.TryGetValue(arg.Message.CorrelationId, out var tcs))
        {
            tcs.TrySetResult(_messageSerializationHelper.Deserialize(arg.Message.Subject, arg.Message.Body.ToString()));
        }

        return Task.CompletedTask;
    }

    public async Task<object> SendMessageToAndWait<TActor, TMessage>(string actorId, TMessage message, CancellationToken cancellationToken)
    {
        var ttl = TimeSpan.FromSeconds(15);
        string messageid = Guid.NewGuid().ToString("N");
        string label = _messageSerializationHelper.ResolveLabel(typeof(TMessage));

        var msg = new ServiceBusMessage()
        {
            Subject = label,
            Body = new BinaryData(_messageSerializationHelper.Serialize(message)),
            ContentType = "applciation/json",
            SessionId = actorId,
            PartitionKey = actorId,
            CorrelationId = actorId,
            MessageId = messageid,
            ReplyTo = MeshClientTopic,
            ReplyToSessionId = _replyToSessionId,
            TimeToLive = ttl
        };
        var cancellationTokenSource = new CancellationTokenSource(ttl);
        var taskCompletionSource = new TaskCompletionSource<object>();
        object o;
        try
        {
            _returns.GetOrAdd(messageid, (k) => taskCompletionSource);
            await _topicsFactory.SendMessageAsync(typeof(TActor), msg, cancellationToken);
            cancellationToken.Register(() => taskCompletionSource.TrySetCanceled());
            cancellationTokenSource.Token.Register(() => taskCompletionSource.TrySetCanceled());
            o = await taskCompletionSource.Task;
        }
        finally
        {
            _returns.TryRemove(messageid, out _);
        }

        return o;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _processor.StartProcessingAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _processor.StopProcessingAsync(cancellationToken);
    }
}