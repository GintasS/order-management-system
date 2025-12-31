using CSS.Challenge.Domain.Configuration;
using CSS.Challenge.Domain.Interfaces;
using CSS.Challenge.Domain.Models;
using CSS.Challenge.Domain.Models.Requests;
using CSS.Challenge.Domain.Utilities;

namespace CSS.Challenge.Domain.Services
{
    /// <summary>
    /// Service that sends results to API.
    /// </summary>
    /// <param name="options">Options for creating the POST request.</param>
    public class SubmitOrderResultsService(ApiProcessingOptions options) : ISubmitOrderResultsService
    {
        private readonly HttpHelper _client = new (options.Endpoint, options.Auth);

        public PostOrdersRequest CreatePostRequest(List<OrderStateChangeLog> orderLogs)
        {
            var millisecondsRate = TimeSpan.FromMilliseconds(options.Rate);
            var millisecondsMin = TimeSpan.FromSeconds(options.Min);
            var millisecondsMax = TimeSpan.FromSeconds(options.Max);

            return new PostOrdersRequest(options.TestId, millisecondsRate, millisecondsMin, millisecondsMax, orderLogs);
        }

        public async Task<string> PostOrdersToApi(PostOrdersRequest request)
        {
            var httpRequest = _client.CreateHttpRequestMessage(request);
            var result = await _client.SendHttpPostRequest(httpRequest);
            return result;
        }
    }
}
