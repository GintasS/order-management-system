using CSS.Challenge.Domain.Configuration;
using CSS.Challenge.Domain.Models;

namespace CSS.Challenge.Domain.Interfaces;

public interface IRandomOrderGeneratorService
{
    List<OrderModel> Generate(OrderProcessingOptions options);
}