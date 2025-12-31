using CSS.Challenge.Domain.Models;
using Serilog;
using static CSS.Challenge.Domain.Models.ConcurrentDataStructures;
using OrderStates = CSS.Challenge.Domain.States.OrderStates;

namespace CSS.Challenge.Domain.Threads
{
    /// <summary>
    /// Thread responsible for delivering orders and managing pickup time.
    /// </summary>
    public static class ThreadPickupTime
    {
        public static void ThreadRemovePickupTime()
        {
            Log.Information("[Pickup Time Thread]: Starting pickup time thread.");

            while (IsThereOrdersInSystem)
            {
                foreach (var order in HeaterOrders.Values)
                {
                    RemoveDeliveryTime(order);
                }

                foreach (var order in CoolerOrders.Values)
                {
                    RemoveDeliveryTime(order);
                }

                foreach (var order in ShelfOrders.Values)
                {
                    RemoveDeliveryTime(order);
                }

                Thread.Sleep(1000);
            }

            Log.Information("[Pickup Time Thread]: No more orders to be picked up. Exiting thread.");
        }

        /// <summary>
        /// Removes order's delivery time by one unit.
        /// When Pickup time of order reaches 0, delivers the order.
        /// </summary>
        /// <param name="order">Order model</param>
        private static void RemoveDeliveryTime(OrderModel order)
        {
            if (order.PickupTime - 1 >= 0)
            {
                order.TryReducePickupTimeByOne();
            }
            else if (order.PickupTime <= 0)
            {
                ConcurrentDataStructures.OrderStates.TryGetValue(order.Id, out var context);
                context.TransitionTo(OrderStates.PickOrderStateInstance);
                context.ExecuteState(order);
            }
        }
    }
}