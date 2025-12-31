using CSS.Challenge.Domain.Models;
using CSS.Challenge.Domain.Models.Enums;
using CSS.Challenge.Domain.Utilities;
using Serilog;

namespace CSS.Challenge.Domain.States;

public class MoveOrderState : State
{
    public MoveOrderState(List<Type> allowedTransitionStates) : base(allowedTransitionStates)
    {
        allowedTransitionStates.Add(this.GetType());
    }

    public override bool TryHandleOrder(OrderModel order)
    {
        Log.Information($"[Order Transition]: Order Id {order.Id} Location: {order.CurrentLocation} Has Transitioned to {Context.LastState?.GetType().Name ?? "N/A"}!");
        PerformActionsWhenRoutingToShelfFailed(order);
        return true;
    }

    /// <summary>
    /// Performs move/discard an order actions when shelf is full.
    /// </summary>
    /// <remarks>This method tries to relocate existing orders to make space on the shelf and
    /// retries adding the specified order.</remarks>
    /// <param name="newOrder">Order model</param>
    private void PerformActionsWhenRoutingToShelfFailed(OrderModel newOrder)
    {
        // Relocation Strategy #1: tries to relocate a hot/cold order from the shelf to an ideal place.

        var status = TryRelocateExistingColdHotOrderFromShelf();
        if (status is false)
        {
            Log.Information("[Main Thread]: Found NO HOT/COLD order on a SHELF to relocate.");

            // If there is no hot/cold order from the shelf, move to Strategy #2.
            // Relocation Strategy #2: Remove the longest pickup item on the shelf with room temp.
            HandleLongestPickupTimeRoomTempOrderOnShelf();
        }


        // One way or another, we have succeeded moving/discarding an order,
        // thus adding this new incoming order
        if (ConcurrentDataStructures.TryAddOrderToShelf(newOrder))
        {
            Log.Information($"[Main Thread]: After RELOCATING items, added Order to shelf. Order Id: {newOrder.Id} Location: {newOrder.CurrentLocation} to SHELF.");
            OrderStateChangeLogHelper.AddPlaceActionToOrder(newOrder, OrderStoragePlaceEnum.Shelf);
        }
    }

    /// <summary>
    /// Remove an order with the longest pickup time on a shelf with room temp.
    /// </summary>
    private void HandleLongestPickupTimeRoomTempOrderOnShelf()
    {
        var shelfOrderLongestPickupTime = HighestPickupTimeDictionary.GetRoomTemperatureOrder();

        if (shelfOrderLongestPickupTime is not null)
        {
            RemoveLongestPickupItemFromShelf(shelfOrderLongestPickupTime);
        }
    }

    private void RemoveLongestPickupItemFromShelf(OrderModel order)
    {
        Log.Information($"[Main Thread]: Discarding Highest Pickup Order from the shelf, Order Id: {order.Id} Location: {order.CurrentLocation}.");

        if (ConcurrentDataStructures.TryRemoveOrderFromShelf(order))
        {
            OrderStateChangeLogHelper.AddDiscardActionToOrder(order);
        }
    }

    /// <summary>
    /// Attempts to relocate the existing hot or cold order with the latest pickup time from the shelf.
    /// </summary>
    /// <returns>true if an order was found and relocated, else false.</returns>
    private bool TryRelocateExistingColdHotOrderFromShelf()
    {
        var hotOrColdOrderOnShelf = HighestPickupTimeDictionary.GetHotColdTemperatureOrder();

        if (hotOrColdOrderOnShelf is not null)
        {
            MoveHotColdOrderFromShelf(hotOrColdOrderOnShelf);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to relocate a hot or cold order from a shelf to its ideal storage location, or discards the order
    /// if relocation is not possible.
    /// </summary>
    /// <param name="hotOrColdOrderOnShelf">Hot/Cold order on the shelf with the longest pickup time.</param>
    private void MoveHotColdOrderFromShelf(OrderModel hotOrColdOrderOnShelf)
    {
        Log.Warning($"[Main Thread]: Found HOT/COLD order on a SHELF to relocate, Order Id: {hotOrColdOrderOnShelf.Id} Location: {hotOrColdOrderOnShelf.CurrentLocation} pickup time: {hotOrColdOrderOnShelf.PickupTime}");

        var status = ConcurrentDataStructures.TryStoreOrderAtIdealPlace(hotOrColdOrderOnShelf);
        if (status)
        {
            Log.Warning($"[Main Thread]: Moved HOT/COLD order, Order Id: {hotOrColdOrderOnShelf.Id} Location: {hotOrColdOrderOnShelf.CurrentLocation} from " +
                        $"shelf to an ideal location successfully.");
            OrderStateChangeLogHelper.AddMoveActionToOrder(hotOrColdOrderOnShelf, hotOrColdOrderOnShelf.CurrentLocation);
        }
        else
        {
            Log.Information($"[Main Thread]: Could not move HOT/COLD order from shelf to ideal location. " +
                            $"Discarding hot/cold item with longest pickup time, Order Id: {hotOrColdOrderOnShelf.Id} Location: {hotOrColdOrderOnShelf.CurrentLocation}.");
            // because degradation is twice as fast on the shelf for a hold/cold item.
            OrderStateChangeLogHelper.AddDiscardActionToOrder(hotOrColdOrderOnShelf);
        }

        ConcurrentDataStructures.TryRemoveOrderFromShelf(hotOrColdOrderOnShelf);
    }

}