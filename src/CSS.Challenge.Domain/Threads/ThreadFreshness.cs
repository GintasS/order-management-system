using CSS.Challenge.Domain.Models;
using Serilog;
using static CSS.Challenge.Domain.Models.ConcurrentDataStructures;

namespace CSS.Challenge.Domain.Threads
{
    /// <summary>
    /// Thread that removes order freshness.
    /// </summary>
    public static class ThreadFreshness
    {
        public static void ThreadRemoveFreshness()
        {
            Log.Information("[Freshness Thread]: Removing freshness thread has started.");
            while (IsThereOrdersInSystem)
            {
                foreach (var order in HeaterOrders.Values)
                {
                    DegradeOrder(order);
                }

                foreach (var order in CoolerOrders.Values)
                {
                    DegradeOrder(order);
                }

                foreach (var order in ShelfOrders.Values)
                {
                    DegradeOrder(order);
                }

                Thread.Sleep(1000);
            }
            Log.Information("[Freshness Thread]: Removing freshness thread has ended.");
        }

        private static void DegradeOrder(OrderModel order)
        {
            if (order.IsStoredAtIdealPlace)
            {
                var status = order.TryReduceFreshnessByOne();
                if (status)
                {
                    //Log.Information($"[Freshness Thread]: Order Id: {order.Id} has removed one freshness point.");
                }
            }
            else
            {
                var status = order.TryReduceFreshnessByTwo();
                if (status)
                {
                    //Log.Information($"[Freshness Thread]: Order Id: {order.Id} has removed TWO freshness points.");
                }
            }

            if (order.IsOrderStale)
            {
                InformFoodIsStale(order);
            }
        }

        private static void InformFoodIsStale(OrderModel order)
        {
            Log.Error($"[Freshness Thread]: Order Id: {order.Id} has reached zero freshness.");
        }
    }
}
