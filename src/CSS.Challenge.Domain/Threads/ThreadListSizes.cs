using CSS.Challenge.Domain.Models;
using Serilog;
using static CSS.Challenge.Domain.Models.ConcurrentDataStructures;

namespace CSS.Challenge.Domain.Threads
{
    /// <summary>
    /// Helper thread to print information about the concurrent data structures.
    /// </summary>
    public static class ThreadListSizes
    {
        public static void ThreadPrintListSizes()
        {
            Log.Information("[Print List Sizes Thread]: Starting thread.");

            while (IsThereOrdersInSystem)
            {
                var roomTempShelfItem = HighestPickupTimeDictionary.GetRoomTemperatureOrder();
                var hotOrColdShelfItems = HighestPickupTimeDictionary.GetAllHotColdOrders();
                var latestHotColdOrder = HighestPickupTimeDictionary.GetHotColdTemperatureOrder()?.Id ?? "N/A";
                Log.Information($"[Print List Sizes Thread]: Heater Orders: {HeaterOrders.Count}, Freezer Orders: {CoolerOrders.Count}, Shelf Orders: {ShelfOrders.Count}");
                Log.Information($"[Print List Sizes Thread]: HotOrColdShelfItems: {hotOrColdShelfItems.Count()} Latest Order: {latestHotColdOrder} Highest Pickup Item On Shelf: {roomTempShelfItem?.Id} {roomTempShelfItem?.PickupTime}");

                Thread.Sleep(500);
            }
            Log.Information("[Print List Sizes Thread]: Ending thread.");
        }
    }
}
