using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.Messages.Dtos
{
    [MessageLabel("balanced-need")]
    public class BalancedNeed2Dto
    {
        public string Key { get; set; }

        public int Quantity { get; set; }
    }
}