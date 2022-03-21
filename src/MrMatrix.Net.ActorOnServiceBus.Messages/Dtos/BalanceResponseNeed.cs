namespace MrMatrix.Net.ActorOnServiceBus.Messages.Dtos
{
    public class BalanceResponseNeed
    {
        public string Key { get; set; }
        public int Donation { get; set; }
        public int Necessity { get; set; }
    }
}