using MrMatrix.Net.ActorOnServiceBus.Conventions;

namespace MrMatrix.Net.ActorOnServiceBus.Messages.Dtos
{
    [MessageLabel("donation")]
    public class DonationDto
    {
        public string DonorId { get; set; }
        public string Key { get; set; }
        public int Quantity { get; set; }
    }
}