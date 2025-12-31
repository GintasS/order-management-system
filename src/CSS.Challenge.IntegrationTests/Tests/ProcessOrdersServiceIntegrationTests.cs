using System.Collections;
using System.Collections.Concurrent;
using CSS.Challenge.Domain.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using CSS.Challenge.Domain.Models;
using CSS.Challenge.Domain.Models.Enums;
using CSS.Challenge.Domain.Services;
using CSS.Challenge.Domain.Utilities;
using Xunit;
using CSS.Challenge.Domain.Interfaces;

namespace CSS.Challenge.IntegrationTests.Tests
{
    public class TestLogSink : ILogEventSink
    {
        private readonly List<LogEvent> _events = new();

        public IReadOnlyList<LogEvent> Events => _events;

        public void Emit(LogEvent logEvent)
        {
            _events.Add(logEvent);
        }

        public void Clear() => _events.Clear();
    }

    public class OrderTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new OrderModel( "O-1", "a", OrderTypeEnum.Hot, 0, 0 )};
            yield return new object[] { new OrderModel("O-2", "a", OrderTypeEnum.Cold, 0, 0) };
            yield return new object[] { new OrderModel("O-3", "a", OrderTypeEnum.Room, 0, 0) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ProcessOrdersServiceIntegrationTests
    {
        private readonly IProcessOrdersService _service;
        private readonly TestLogSink _logSink;
        private readonly OrderProcessingOptions _options;

        public ProcessOrdersServiceIntegrationTests()
        {
            _service = new ProcessOrdersService();
            _options = new OrderProcessingOptions(500, 6, 6, 6, 4, 7, 4, 8, 1);

            _logSink = new TestLogSink();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Sink(_logSink)
                .CreateLogger();

            ConcurrentDataStructures.ShelfOrders.Clear();
            ConcurrentDataStructures.CoolerOrders.Clear();
            ConcurrentDataStructures.HeaterOrders.Clear();
            OrderStateChangeLogHelper.AllActions = new BlockingCollection<OrderStateChangeLog>();

            _logSink.Clear();
        }

        [Theory]
        [ClassData(typeof(OrderTestData))]
        public void HandleStoreOrderOperationsForMainOrderTypesHotColdRoom(OrderModel order)
        {
            // Arrange
            var orders = new List<OrderModel>() { order };

            // Act
            _service.ProcessOrders(orders, _options);

            // Assert
            // Assert 1: order is in ideal place (heater)
            if (order.OrderType == OrderTypeEnum.Hot)
            {
                Assert.Contains(order, ConcurrentDataStructures.HeaterOrders.Values);
                Assert.Equal(OrderStoragePlaceEnum.Heater, order.CurrentLocation);
            }
            else if (order.OrderType == OrderTypeEnum.Cold)
            {
                Assert.Contains(order, ConcurrentDataStructures.CoolerOrders.Values);
                Assert.Equal(OrderStoragePlaceEnum.Cooler, order.CurrentLocation);
            }
            else if (order.OrderType == OrderTypeEnum.Room)
            {
                Assert.Contains(order, ConcurrentDataStructures.ShelfOrders.Values);
                Assert.Equal(OrderStoragePlaceEnum.Shelf, order.CurrentLocation);
            }

            // Assert 2: place action logged
            var placeLog = OrderStateChangeLogHelper.AllActions
                    .SingleOrDefault(x => x.Id == order.Id);
            Assert.Equal(order.Id, placeLog.Id);
            Assert.Equal(order.CurrentLocation.ToString().ToLower(), placeLog.Target);

            // Assert 3: success log message written
            var logMessage = _logSink.Events
                .Select(e => e.RenderMessage())
                .Single(msg => msg.Contains($"[Main Thread]: Order Id: {order.Id} Location: {order.CurrentLocation} routed to ideal location successfully."));
            
            Assert.Contains($"[Main Thread]: Order Id: {order.Id} Location: {order.CurrentLocation} routed to ideal location successfully.", logMessage);
        }

        [Fact]
        public void HandleStoreOrderOperations_UnknownTemp_LogsNonIdealPath_DoesNotStoreInIdealPlaces()
        {
            // Arrange
            var newOrder = new OrderModel("O-4", "a", OrderTypeEnum.Unknown, 0, 10);

            var orders = new List<OrderModel>() { newOrder };

            // Act
            _service.ProcessOrders(orders, _options);

            // Assert:
            // Assert 1: order not stored in heater or freezer.
            var firstOrder = orders[0];
            Assert.DoesNotContain(firstOrder, ConcurrentDataStructures.HeaterOrders.Values);
            Assert.DoesNotContain(firstOrder, ConcurrentDataStructures.CoolerOrders.Values);
            Assert.DoesNotContain(firstOrder, ConcurrentDataStructures.ShelfOrders.Values);

            // Assert 2: NON ideal log message exists
            Assert.Contains(
                _logSink.Events.Select(e => e.RenderMessage()),
                msg => msg.Contains($"[Main Thread]: Order Id: {firstOrder.Id} is routed to NON ideal location! Trying to add it to a shelf."));

            Assert.Contains(
                _logSink.Events.Select(e => e.RenderMessage()),
                msg => msg.Contains($"[Main Thread]: Unknown Order Id: {firstOrder.Id} temperature type detected: {firstOrder.OrderType}."));

            // Assert 3: Since ideal path failed, order should be on the shelf.
            Assert.DoesNotContain(OrderStateChangeLogHelper.AllActions, x => x.Id == "O-4");
        }
    }

}
