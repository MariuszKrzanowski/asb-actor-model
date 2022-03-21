using System.Collections.Generic;

namespace MrMatrix.Net.ActorOnServiceBus.Actors.Sagas
{
    public class NeedsBalancerSaga
    {
        public string Key { get; set; }

        public List<RegisteredNeed> Necessities { get; set; } = new List<RegisteredNeed>();
        public List<RegisteredNeed> Donations { get; set; } = new List<RegisteredNeed>();
    }
}