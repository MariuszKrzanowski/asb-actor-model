using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.Messages.Dtos
{
    [MessageLabel("balance")]
    public class BalanceDto
    {
        public string Key { get; set; }
        public int Entered { get; set; }
        public int Balanced { get; set; }
    }
}