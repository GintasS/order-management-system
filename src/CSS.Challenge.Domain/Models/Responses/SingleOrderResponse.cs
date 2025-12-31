namespace CSS.Challenge.Domain.Models.Responses
{
    public class SingleOrderResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Temp { get; set; }
        public long Price { get; set; }
        public long Freshness { get; set; }
    }
}
