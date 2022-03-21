using System.Collections.Generic;

namespace MrMatrix.Net.ActorOnServiceBus.Actors.Sagas
{
    public class NecessitousSaga
    {
        public string NecessitousId { get; set; }
        public Dictionary<string, NeedBalance> Balance { get; set; } = new Dictionary<string, NeedBalance>();
    }
}