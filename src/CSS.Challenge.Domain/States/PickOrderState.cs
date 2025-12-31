using CSS.Challenge.Domain.Models;
using CSS.Challenge.Domain.Utilities;
using Serilog;

namespace CSS.Challenge.Domain.States;

public class PickOrderState : State
{
    public PickOrderState(List<Type> allowedTransitionStates) : base(allowedTransitionStates)
    {
        allowedTransitionStates.Add(this.GetType());
    }

    public override bool TryHandleOrder(OrderModel order)
    {
        Log.Information($"[Order Transition]: Order Id: {order.Id} Location: {order.CurrentLocation} Has Transitioned to {Context.LastState?.GetType().Name ?? "N/A"}!");
        DecideOnOrder(order);
        return true;
    }

    /// <summary>
    /// Method that will decide if we are discarding or giving order to a delivery person.
    /// </summary>
    /// <param name="order">Order model></param>
    private void DecideOnOrder(OrderModel order)
    {
        Log.Information($"[Pickup Time Thread]: Order Id: {order.Id} Location: {order.CurrentLocation} is ready for pickup! Checking freshness.");
        if (order.Freshness > 0)
        {
            Log.Information($"[Pickup Time Thread]: Order Id: {order.Id} Location: {order.CurrentLocation} is given to the delivery person.");
            OrderStateChangeLogHelper.AddPickupActionToOrder(order);
            ConcurrentDataStructures.RemoveOrderFromDataStructures(order);
        }
        else
        {
            Console.WriteLine("TRANSITIONING TO DISCARD STATE");
            Context.TransitionTo(OrderStates.DiscardOrderStateInstance);
            Context.ExecuteState(order);
        }
    }
}