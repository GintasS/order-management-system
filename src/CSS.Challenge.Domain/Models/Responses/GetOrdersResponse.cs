namespace CSS.Challenge.Domain.Models.Responses
{
    public class GetOrdersResponse
    {
        public string TestId { get; set; }

        public List<OrderModel> Orders { get; set; }

        public GetOrdersResponse(string testId, List<SingleOrderResponse> orders)
        {
            TestId = testId;
            Orders = orders.Select(x => new OrderModel(x)).ToList();
        }
    }
}
