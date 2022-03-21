using System.Collections.Generic;
using MrMatrix.Net.ActorOnServiceBus.Messages.Dtos;

namespace MrMatrix.Net.ActorOnServiceBus.Actors.Sagas
{
    public class BalancedNeed
    {
        public string NecessitiousId { get; set; }


        public int Quantity { get; set; }

        public List<MatchedDonation> Donations { get; set; }
    }
}