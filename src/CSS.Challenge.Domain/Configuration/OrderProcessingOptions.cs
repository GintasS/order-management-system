namespace CSS.Challenge.Domain.Configuration
{
    public class OrderProcessingOptions
    {
        public int Rate { get; set; }
        public int HeaterCapacity { get; set; }
        public int CoolerCapacity { get; set; }
        public int ShelfCapacity { get; set; }
        public int FreshnessMin { get; set; }
        public int FreshnessMax { get; set; }
        public int PickupTimeMin { get; set; }
        public int PickupTimeMax { get; set; }
        public int OrderCount { get; set; }

        public OrderProcessingOptions(int rate, int heaterCapacity, int coolerCapacity, int shelfCapacity, int pickupTimeMin, int pickupTimeMax, int freshnessMin, int freshnessMax, int orderCount)
        {
            Rate = rate;
            HeaterCapacity = heaterCapacity;
            CoolerCapacity = coolerCapacity;
            ShelfCapacity = shelfCapacity;
            PickupTimeMin = pickupTimeMin;
            PickupTimeMax = pickupTimeMax;
            FreshnessMin = freshnessMin;
            FreshnessMax = freshnessMax;
            OrderCount = orderCount;
        }
    }

}
