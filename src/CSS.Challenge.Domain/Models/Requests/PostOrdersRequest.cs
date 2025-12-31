using System.Text.Json.Serialization;

namespace CSS.Challenge.Domain.Models.Requests
{
    public class Options(TimeSpan rate, TimeSpan min, TimeSpan max)
    {
        [JsonPropertyName("rate")]
        public long Rate { get; init; } = (long)rate.TotalMicroseconds;
        [JsonPropertyName("min")]
        public long Min { get; init; } = (long)min.TotalMicroseconds;
        [JsonPropertyName("max")]
        public long Max { get; init; } = (long)max.TotalMicroseconds;
    };

    public class Solution(Options options, List<OrderStateChangeLog> actions)
    {
        [JsonPropertyName("options")]
        public Options Options { get; init; } = options;
        [JsonPropertyName("actions")]
        public List<OrderStateChangeLog> Actions { get; init; } = actions;
    }

    public class PostOrdersRequest
    {
        public string TestId;
        [JsonPropertyName("options")] 
        public Options Options;
        [JsonPropertyName("actions")]
        public List<OrderStateChangeLog> Actions;


        public PostOrdersRequest(string testId, TimeSpan rate, TimeSpan min, TimeSpan max, List<OrderStateChangeLog> actions)
        {
            TestId = testId;
            Options = new Options(rate, min, max);
            Actions = actions;
        }
    }
}
