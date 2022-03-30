using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.Messages.Dtos
{
    [MessageLabel("balanced-need")]
    public class BalancedNeedDto
    {
        public string Key { get; set; }
        public int Quantity { get; set; }
    }
}