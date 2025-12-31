using CSS.Challenge.Domain.Models.Responses;

namespace CSS.Challenge.Domain.Interfaces
{
    public interface IReceiveOrderService
    {
        Task<GetOrdersResponse> GetOrdersFromApi();
    }
}
