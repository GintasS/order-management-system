using CSS.Challenge.Domain.Models;
using Serilog;

namespace CSS.Challenge.Domain.States;

/// <summary>
/// A cooking state for the order that cooks the order instantly.
/// </summary>
public class CookOrderState : State
{
    public CookOrderState(List<Type> allowedTransitionStates) : base(allowedTransitionStates)
    {
        allowedTransitionStates.Add(this.GetType());
    }

    public override bool TryHandleOrder(OrderModel order)
    {
        Log.Information($"[Order Transition]: Order Id: {order.Id} Location: {order.CurrentLocation} Has Transitioned to {Context.LastState?.GetType().Name ?? "N/A"}!");
        // As we don't have cooking logic, returning always true.
        return true;
    }

}