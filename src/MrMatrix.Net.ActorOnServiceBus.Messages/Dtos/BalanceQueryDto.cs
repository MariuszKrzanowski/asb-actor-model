using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.Messages.Dtos
{
    [MessageLabel("balance-query")]
    public class BalanceQueryDto
    {
        public string PersonId { get; set; }
    }
}