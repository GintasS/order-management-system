using System.Collections.Concurrent;
using CSS.Challenge.Domain.Configuration;
using CSS.Challenge.Domain.Models.Enums;
using CSS.Challenge.Domain.States;
using Serilog;

namespace CSS.Challenge.Domain.Models
{
    /// <summary>
    /// Static class that holds most of the concurrent data structures (except for HighestPickupTimeDictionary).
    /// </summary>
    public static class ConcurrentDataStructures
    {
        public static ConcurrentDictionary<string, OrderModel> CoolerOrders { get; }= new();
        public static ConcurrentDictionary<string, OrderModel> HeaterOrders { get; } = new();
        public static ConcurrentDictionary<string, OrderModel> ShelfOrders { get; } = new();

        public static ConcurrentDictionary<string, Context> OrderStates { get; } = new();

        private static int _heaterCapacity = 6;
        private static int _coolerCapacity = 6;
        private static int _shelfCapacity = 12;

        public static bool IsThereOrdersInSystem => HeaterOrders.Count + CoolerOrders.Count + ShelfOrders.Count > 0;

        public static void InitializeCapacities(OrderProcessingOptions options)
        {
            _heaterCapacity = options.HeaterCapacity;
            _coolerCapacity = options.CoolerCapacity;
            _shelfCapacity = options.ShelfCapacity;
        }

        public static bool TryStoreOrderAtIdealPlace(OrderModel order)
        {
            switch (order.OrderType)
            {
                case OrderTypeEnum.Hot:
                    return TryAddOrderToHeater(order);
                case OrderTypeEnum.Cold:
                    return TryAddOrderToCooler(order);
                case OrderTypeEnum.Room:
                    return TryAddOrderToShelf(order);
                case OrderTypeEnum.Unknown:
                    Log.Warning($"[Main Thread]: Unknown Order Id: {order.Id} temperature type detected: {order.OrderType}.");
                    break;
            }
            return false;
        }

        public static void RemoveOrderFromDataStructures(OrderModel order)
        {
            switch (order.CurrentLocation)
            {
                case OrderStoragePlaceEnum.Heater:
                {
                    var status = HeaterOrders.TryRemove(order.Id, out _);
                    break;
                }
                case OrderStoragePlaceEnum.Cooler:
                {
                    var status = CoolerOrders.TryRemove(order.Id, out _);
                    break;
                }
                case OrderStoragePlaceEnum.Shelf:
                {
                    var status = ShelfOrders.TryRemove(order.Id, out _);
                    break;
                }
            }

            HighestPickupTimeDictionary.TryRemoveOrder(order);
        }

        private static bool TryAddOrderToHeater(OrderModel order)
        {
            if (HeaterOrders.Count + 1 > _heaterCapacity)
            {
                return false;
            }

            order.SetOrderLocation(OrderStoragePlaceEnum.Heater);
            HeaterOrders.TryAdd(order.Id, order);
            return true;
        }


        private static bool TryAddOrderToCooler(OrderModel order)
        {
            if (CoolerOrders.Count + 1 > _coolerCapacity)
            {
                return false;
            }

            order.SetOrderLocation(OrderStoragePlaceEnum.Cooler);
            CoolerOrders.TryAdd(order.Id, order);
            return true;
        }

        public static bool TryAddOrderToShelf(OrderModel order)
        {
            if (ShelfOrders.Count + 1 > _shelfCapacity || order.OrderType is OrderTypeEnum.Unknown)
            {
                return false;
            }

            AddOrderToShelf(order);
            return true;
        }

        private static void AddOrderToShelf(OrderModel order)
        {
            order.SetOrderLocation(OrderStoragePlaceEnum.Shelf);
            ShelfOrders.TryAdd(order.Id, order);
            HighestPickupTimeDictionary.TryAddOrder(order);
        }


        public static bool TryRemoveOrderFromShelf(OrderModel order)
        {
            if (ShelfOrders.Count - 1 < 0)
            {
                return false;
            }

            ShelfOrders.TryRemove(order.Id, out _);
            HighestPickupTimeDictionary.TryRemoveOrder(order);
            return true;
        }
    }
    
}
