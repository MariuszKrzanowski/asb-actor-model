using MrMatrix.Net.ActorOnServiceBus.Conventions;
using System.Collections.Generic;

namespace MrMatrix.Net.ActorOnServiceBus.Messages.Dtos
{
    [MessageLabel("balance-response")]
    public class BalanceResponseDto
    {
        public string PersonId { get; set; }
        public List<BalanceResponseNeed> Balance { get; set; }
    }
}