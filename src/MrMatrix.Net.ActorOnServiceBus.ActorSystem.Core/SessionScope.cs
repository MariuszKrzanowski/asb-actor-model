using Azure.Messaging.ServiceBus;

namespace MrMatrix.Net.ActorOnServiceBus.ActorSystem.Core;

public class SessionScope
{
    public class MessageToBeSent
    {
        public ServiceBusMessage Message { get; set; }
        public string Topic { get; set; }
    }

    private List<MessageToBeSent> _outbox = new List<MessageToBeSent>();

    public IEnumerable<MessageToBeSent> ReadOutbox()
    {
        return _outbox;
    }

    public string? SessionId { get; set; }

    public ProcessSessionMessageEventArgs? ProccessedMessageContext { get; set; }

    public void PlaceInOutbox(string topic, ServiceBusMessage msg)
    {
        _outbox.Add(new MessageToBeSent() { Topic = topic, Message = msg });
    }

    public void ClearOutbox()
    {
        _outbox.Clear();
    }
}