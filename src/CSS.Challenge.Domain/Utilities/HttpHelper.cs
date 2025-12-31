using CSS.Challenge.Domain.Models.Requests;
using CSS.Challenge.Domain.Models.Responses;
using Serilog;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace CSS.Challenge.Domain.Utilities
{
    public class HttpHelper(string endpoint, string auth)
    {
        private readonly HttpClient _client = new();

        /// <summary>
        ///  NewProblemAsync fetches a new test problem from the server. The URL also works in a browser for convenience.
        /// </summary>
        public async Task<GetOrdersResponse> NewProblemAsync(string name, long seed = 0)
        {
            if (seed == 0)
            {
                seed = new Random().NextInt64();
                Log.Information(seed.ToString());
            }

            var url = $"{endpoint}/interview/challenge/new?auth={auth}&name={name}&seed={seed}";
            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"{url}: {response.StatusCode}");
            }

            var id = response.Headers.GetValues("x-test-id").First();
            Log.Information($"Fetched new test problem, id={id}: {url}");

            var orders = await response.Content.ReadFromJsonAsync<List<SingleOrderResponse>>();
            Log.Information($"Received {orders?.Count ?? 0} orders");

            return new GetOrdersResponse(id, orders ?? []);
        }

        /// <summary>
        /// SolveAsync submits a sequence of actions and parameters as a solution to a test problem. Returns test result.
        /// </summary>
        public HttpRequestMessage CreateHttpRequestMessage(PostOrdersRequest request)
        {
            var solution = new Solution(request.Options, request.Actions.ToList());

            var url = $"{endpoint}/interview/challenge/solve?auth={auth}";
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);

            httpRequest.Headers.Add("x-test-id", request.TestId);
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(solution), Encoding.UTF8, "application/json");
            return httpRequest;
        }

        public async Task<string> SendHttpPostRequest(HttpRequestMessage httpRequest)
        {
            var response = await _client.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"{httpRequest.RequestUri}: {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
