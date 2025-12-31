using System.Collections.Concurrent;
using CSS.Challenge.Domain.Models.Enums;

namespace CSS.Challenge.Domain.Models
{
    public static class HighestPickupTimeDictionary
    {
        private static readonly ConcurrentDictionary<string, OrderModel> ShelfOrders = new();

        private static readonly string RoomTempOrderKey = "room_temp_shelf_item_highest_pickup_time";

        private static readonly string HotColdTempOrderKey = "hot_cold_temp_shelf_item_highest_pickup_time";


        public static IEnumerable<OrderModel> GetAllHotColdOrders()
        {
            return ShelfOrders.Values.Where(x => x.OrderType is OrderTypeEnum.Hot or OrderTypeEnum.Cold);
        }

        public static void TryAddOrder(OrderModel order)
        {
            switch (order.OrderType)
            {
                case OrderTypeEnum.Hot or OrderTypeEnum.Cold:
                    ShelfOrders.TryAdd(order.Id, order);
                    TrySetHighestPickupTimeForTempType(order, HotColdTempOrderKey);
                    return;
                case OrderTypeEnum.Room:
                    ShelfOrders.TryAdd(order.Id, order);
                    TrySetHighestPickupTimeForTempType(order, RoomTempOrderKey);
                    return;
                default:
                    return;
            }
        }

        public static OrderModel GetRoomTemperatureOrder()
        {
            ShelfOrders.TryGetValue(RoomTempOrderKey, out var order);
            return order;
        }

        public static OrderModel GetHotColdTemperatureOrder()
        {
            ShelfOrders.TryGetValue(HotColdTempOrderKey, out var order);
            return order;
        }

        public static void TrySetHighestPickupTimeForTempType(OrderModel newOrder, string dictKey)
        {
            ShelfOrders.TryGetValue(dictKey, out var currentDictOrder);
            if (currentDictOrder != null && newOrder.PickupTime >= currentDictOrder.PickupTime)
            {
                ShelfOrders.TryUpdate(dictKey, newOrder, currentDictOrder);
            }
            else if (currentDictOrder == null)
            {
                ShelfOrders.TryAdd(dictKey, newOrder);
            }
        }

        public static void TryRemoveOrder(OrderModel orderToRemove)
        {
            if (ShelfOrders.TryRemove(orderToRemove.Id, out _))
            {
                if (TryRemoveHighestPickupTimeOrder(orderToRemove, HotColdTempOrderKey))
                {
                    TrySetNewHighestPickupTimeOrderForTempType([OrderTypeEnum.Hot, OrderTypeEnum.Cold], HotColdTempOrderKey);
                }

                if (TryRemoveHighestPickupTimeOrder(orderToRemove, RoomTempOrderKey))
                {
                    TrySetNewHighestPickupTimeOrderForTempType([OrderTypeEnum.Room], RoomTempOrderKey);
                }
            }
        }

        private static bool TryRemoveHighestPickupTimeOrder(OrderModel orderToRemove, string key)
        {
            ShelfOrders.TryGetValue(key, out var highestPickupTimeOrder);
            
            if (highestPickupTimeOrder?.Id == orderToRemove.Id)
            {
                ShelfOrders.TryRemove(key, out _);
                return true;
            }

            return false;
        }

        private static void TrySetNewHighestPickupTimeOrderForTempType(HashSet<OrderTypeEnum> tempTypes, string key)
        {
            var orderByTempType = ShelfOrders.Values
                .OrderByDescending(x => x.PickupTime)
                .FirstOrDefault(x => tempTypes.Contains(x.OrderType));

            if (orderByTempType is not null)
            {
                ShelfOrders.TryAdd(key, orderByTempType);
            }
        }
    }
}
