namespace CSS.Challenge.Domain.Configuration
{
    public class ApiProcessingOptions
    {
        public string Auth { get; set; }
        public string Endpoint { get; set; }
        public string Name { get; set; }
        public long Seed { get; set; }
        public int Rate { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string TestId { get; set; }
    }
}
