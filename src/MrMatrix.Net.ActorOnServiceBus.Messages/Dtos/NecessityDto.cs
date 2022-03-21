using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.Messages.Dtos
{
    [MessageLabel("necessity")]
    public class NecessityDto
    {
        public string NecessitouId { get; set; }
        public string Key { get; set; }
        public int Quantity { get; set; }
    }
}