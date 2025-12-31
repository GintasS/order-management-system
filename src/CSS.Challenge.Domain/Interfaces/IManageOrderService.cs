using CSS.Challenge.Domain.Configuration;
using CSS.Challenge.Domain.Models;

namespace CSS.Challenge.Domain.Interfaces
{
    public interface IProcessOrdersService
    {
        Task<List<OrderStateChangeLog>> ProcessOrders(List<OrderModel> orders, OrderProcessingOptions options);
    }
}
