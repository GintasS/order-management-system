namespace CSS.Challenge.Web.Models
{
    public class StartProcessingRequest
    {
        public int HeaterCapacity { get; set; }
        public int CoolerCapacity { get; set; }
        public int ShelfCapacity { get; set; }
        public int FreshnessMin { get; set; } = 1;
        public int FreshnessMax { get; set; } = 100;
        public int PickupTimeMin { get; set; } = 4;
        public int PickupTimeMax { get; set; } = 20;
        public int OrderCount { get; set; } = 10;
    }
}
