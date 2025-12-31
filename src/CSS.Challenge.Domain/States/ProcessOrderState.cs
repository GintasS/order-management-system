using CSS.Challenge.Domain.Configuration;
using CSS.Challenge.Domain.Models;
using CSS.Challenge.Domain.Utilities;
using Serilog;

namespace CSS.Challenge.Domain.States;

/// <summary>
/// This state pre-processes a single order before giving it back to other states.
/// </summary>
public class PreprocessSingleOrderState : State
{
    private OrderProcessingOptions _options;

    public PreprocessSingleOrderState(List<Type> allowedTransitionStates) : base(allowedTransitionStates)
    {
        allowedTransitionStates.Add(this.GetType());
    }

    public void SetProcessingOptions(OrderProcessingOptions newOptions)
    {
        _options = newOptions;
    }

    public override bool TryHandleOrder(OrderModel order)
    {
        Log.Information($"[Order Transition]: Order Id: {order.Id} Type: {order.OrderType} Has Transitioned to {Context.LastState?.GetType().Name ?? "N/A"}!");
        ProcessSingleOrder(order);
        return true;
    }

    private void ProcessSingleOrder(OrderModel order)
    {
        ConcurrentDataStructures.OrderStates.TryAdd(order.Id, Context);
        
        var generatedRandomDeliveryTime = Helpers.GeneratePickupTime(_options.PickupTimeMin, _options.PickupTimeMax);
        order.SetPickupTime(generatedRandomDeliveryTime);

        Log.Information(
            $"[Main Thread]: Processed Order Id: {order.Id} Type: {order.OrderType} Freshness: {order.Freshness} Pickup Time: {order.PickupTime}");

        Context.TransitionTo(OrderStates.CookOrderStateInstance);
        var status = Context.ExecuteState(order);

        if (status)
        {
            Context.TransitionTo(OrderStates.PlaceOrderStateInstance);
            Context.ExecuteState(order);
        }
        else
        {
            Log.Error($"[Main Thread]: Failed to cook the Order Id: {order.Id} Type: {order.OrderType}");
        }
    }
}