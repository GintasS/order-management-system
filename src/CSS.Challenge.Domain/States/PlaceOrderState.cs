using CSS.Challenge.Domain.Models;
using CSS.Challenge.Domain.Models.Enums;
using CSS.Challenge.Domain.Utilities;
using Serilog;

namespace CSS.Challenge.Domain.States;

public class PlaceOrderState : State
{
    public PlaceOrderState(List<Type> allowedTransitionStates) : base(allowedTransitionStates)
    {
        allowedTransitionStates.Add(this.GetType());
    }

    public override bool TryHandleOrder(OrderModel order)
    {
        Log.Information($"[Order Transition]: Order Id: {order.Id} Type: {order.OrderType} Has Transitioned to {Context.LastState?.GetType().Name ?? "N/A"}!");
        PlaceOrder(order);
        return true;
    }

    /// <summary>
    /// Stores order in a specific data structure (heater, cooler, room) with
    /// specific handling conditions.
    /// </summary>
    /// <param name="newOrder">Order model</param>
    private void PlaceOrder(OrderModel newOrder)
    {
        // Try to store an order at an ideal place.
        var status = ConcurrentDataStructures.TryStoreOrderAtIdealPlace(newOrder);
        if (status)
        {

            OrderStateChangeLogHelper.AddPlaceActionToOrder(newOrder, newOrder.CurrentLocation);
            Log.Information($"[Main Thread]: Order Id: {newOrder.Id} Location: {newOrder.CurrentLocation} routed to ideal location successfully.");
        }
        else
        {
            Log.Information($"[Main Thread]: Order Id: {newOrder.Id} is routed to NON ideal location! " +
                            $"Trying to add it to a shelf.");

            // if not possible, store it at non-ideal place.
            HandleOrderStorageToNotIdealPlace(newOrder);
        }
    }

    /// <summary>
    /// Attempts to place an order onto a shelf, handling a case where shelf is full.
    /// </summary>
    /// <param name="newOrder">Order model</param>
    private void HandleOrderStorageToNotIdealPlace(OrderModel newOrder)
    {
        // Try adding an order to a shelf (since an ideal place is full).
        // This will make the order degrade twice as fast.
        // - This method HandleOrderStorageToNotIdealPlace will also get called
        // when an order is room type and shelf is full, as we need to make space.


        var shelfStatus = ConcurrentDataStructures.TryAddOrderToShelf(newOrder);
        if (shelfStatus is false)
        {
            Log.Information($"[Main Thread]: Order Id: {newOrder.Id} Type: {newOrder.OrderType} couldn't be routed to shelf location either!" +
                            $" Trying to move existing cold/hot to shelf to make space.");

            // If shelf is full, we need to move an order out/discard an order.
            Context.TransitionTo(OrderStates.MoveOrderStateInstance);
            Context.ExecuteState(newOrder);
        }
        else
        {
            Log.Information($"[Main Thread]: Order Id: {newOrder.Id} Location: {newOrder.CurrentLocation} routed to shelf location successfully.");
            OrderStateChangeLogHelper.AddPlaceActionToOrder(newOrder, OrderStoragePlaceEnum.Shelf);
        }
    }
}