using CSS.Challenge.Domain.Models;
using CSS.Challenge.Domain.Models.Requests;

namespace CSS.Challenge.Domain.Interfaces
{
    public interface ISubmitOrderResultsService
    {
        PostOrdersRequest CreatePostRequest(List<OrderStateChangeLog> orderStateChangeLogs);

        Task<string> PostOrdersToApi(PostOrdersRequest request);
    }
}
