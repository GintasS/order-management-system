using CSS.Challenge.Domain.Models.Enums;
using CSS.Challenge.Domain.Models.Responses;

namespace CSS.Challenge.Domain.Models;

public class OrderModel
{
    public string Id { get; set; }

    public string Name { get; set; }

    public OrderTypeEnum OrderType { get; set; }

    public long Price { get; set; }

    public long Freshness { get; set; }

    public int PickupTime { get; set; }

    public bool IsStoredAtIdealPlace => CurrentLocation == _idealStoreLocation;

    private readonly OrderStoragePlaceEnum _idealStoreLocation;

    public OrderStoragePlaceEnum CurrentLocation { get; set; }

    public bool IsOrderStale => Freshness <= 0;

    public OrderModel(string id, string name, OrderTypeEnum type, long price, long freshness)
    {
        Id = id;
        Name = name;
        OrderType = type;
        Price = price;
        Freshness = freshness;
    }

    public OrderModel(SingleOrderResponse singleOrderResponse)
    {
        Id = singleOrderResponse.Id;
        Name = singleOrderResponse.Name;
        Freshness = singleOrderResponse.Freshness;
        Price = singleOrderResponse.Price;
        OrderType = GetOrderTypeFromTemp(singleOrderResponse.Temp);
        _idealStoreLocation = GetIdealPlaceNameForOrder();
    }

    public OrderStoragePlaceEnum GetIdealPlaceNameForOrder()
    {
        return OrderType switch
        {
            OrderTypeEnum.Hot => OrderStoragePlaceEnum.Heater,
            OrderTypeEnum.Cold => OrderStoragePlaceEnum.Cooler,
            OrderTypeEnum.Room => OrderStoragePlaceEnum.Shelf,
            OrderTypeEnum.Unknown => OrderStoragePlaceEnum.Unknown,
            _ => OrderStoragePlaceEnum.Unknown
        };
    }

    private OrderTypeEnum GetOrderTypeFromTemp(string temp)
    {
        switch (temp)
        {
            case "hot":
                return OrderTypeEnum.Hot;
            case "cold":
                return OrderTypeEnum.Cold;
            case "room":
                return OrderTypeEnum.Room;
            default:
                return OrderTypeEnum.Unknown;
        }
    }

    public bool TryReduceFreshnessByOne()
    {
        if (Freshness - 1 < 0)
        {
            return false;
        }

        Freshness -= 1;
        return true;
    }

    public bool TryReduceFreshnessByTwo()
    {
        if (Freshness - 2 < 0)
        {
            return false;
        }

        Freshness -= 2;
        return true;
    }

    public bool TryReducePickupTimeByOne()
    {
        if (PickupTime - 1 < 0)
        {
            return false;
        }

        PickupTime -= 1;
        return true;
    }

    public void SetPickupTime(int newPickupTime)
    {
        PickupTime = newPickupTime;
    }

    public void SetOrderLocation(OrderStoragePlaceEnum newStoragePlace)
    {
        CurrentLocation = newStoragePlace;
    }
}