using CSS.Challenge.Domain.Configuration;
using CSS.Challenge.Domain.Interfaces;
using CSS.Challenge.Domain.Models.Responses;
using CSS.Challenge.Domain.Utilities;

namespace CSS.Challenge.Domain.Services
{
    /// <summary>
    /// Service that receives orders from API.
    /// </summary>
    /// <param name="options">Processing options</param>
    public class ReceiveOrderService(ApiProcessingOptions options) : IReceiveOrderService
    {
        private readonly HttpHelper _client = new (options.Endpoint, options.Auth);

        public async Task<GetOrdersResponse> GetOrdersFromApi()
        {
            var ordersResponse = await _client.NewProblemAsync(options.Name, options.Seed);
            return ordersResponse;
        }
    }
}
