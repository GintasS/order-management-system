using CSS.Challenge.Domain.Models;
using CSS.Challenge.Domain.Utilities;
using Serilog;

namespace CSS.Challenge.Domain.States;

public class DiscardOrderState : State
{
    public DiscardOrderState(List<Type> allowedTransitionStates) : base(allowedTransitionStates)
    {
        allowedTransitionStates.Add(this.GetType());
    }

    public override bool TryHandleOrder(OrderModel order)
    {
        Log.Information($"[Order Transition]: Order Id {order.Id} Location: {order.CurrentLocation} Has Transitioned to {Context.LastState?.GetType().Name ?? "N/A"}!");
        Log.Information($"[Pickup Time Thread]: Order Id {order.Id} {order.CurrentLocation} is discarded due to zero freshness.");
        ConcurrentDataStructures.RemoveOrderFromDataStructures(order);
        OrderStateChangeLogHelper.AddDiscardActionToOrder(order);
        return true;
    }

}