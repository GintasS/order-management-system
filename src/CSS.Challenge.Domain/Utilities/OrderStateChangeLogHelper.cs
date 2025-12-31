using CSS.Challenge.Domain.Models;
using System.Collections.Concurrent;
using CSS.Challenge.Domain.Models.Enums;

namespace CSS.Challenge.Domain.Utilities
{
    public class OrderStateChangeLogHelper
    {
        public static BlockingCollection<OrderStateChangeLog> AllActions { get; set; } = new ();

        public static void AddPlaceActionToOrder(OrderModel order, OrderStoragePlaceEnum location)
        {
            var orderChangeLog = new OrderStateChangeLog(DateTime.Now, order.Id, OrderStateChangeLog.Place, location.ToString().ToLower());
            AllActions.Add(orderChangeLog);
        }

        public static void AddDiscardActionToOrder(OrderModel order)
        {
            var orderChangeLog = new OrderStateChangeLog(DateTime.Now, order.Id, OrderStateChangeLog.Discard, order.CurrentLocation.ToString().ToLower());
            AllActions.Add(orderChangeLog);
        }

        public static void AddPickupActionToOrder(OrderModel order)
        {
            var orderChangeLog = new OrderStateChangeLog(DateTime.Now, order.Id, OrderStateChangeLog.Pickup, order.CurrentLocation.ToString().ToLower());
            AllActions.Add(orderChangeLog);
        }

        public static void AddMoveActionToOrder(OrderModel order, OrderStoragePlaceEnum location)
        {
            var orderChangeLog = new OrderStateChangeLog(DateTime.Now, order.Id, OrderStateChangeLog.Move, location.ToString().ToLower());
            AllActions.Add(orderChangeLog);
        }
    }
}
