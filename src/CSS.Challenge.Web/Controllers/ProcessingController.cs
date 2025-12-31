using CSS.Challenge.Domain;
using CSS.Challenge.Domain.Configuration;
using CSS.Challenge.Domain.Interfaces;
using CSS.Challenge.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;

namespace CSS.Challenge.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessingController : ControllerBase
    {
        [HttpPost("start")]
        public async Task<IActionResult> StartProcessing([FromBody] StartProcessingRequest request)
        {
            var orderProcessingOptions = new OrderProcessingOptions(
                rate: 500,
                heaterCapacity: request.HeaterCapacity,
                coolerCapacity: request.CoolerCapacity,
                shelfCapacity: request.ShelfCapacity,
                pickupTimeMin: request.PickupTimeMin,
                pickupTimeMax: request.PickupTimeMax,
                freshnessMin: request.FreshnessMin,
                freshnessMax: request.FreshnessMax,
                orderCount: request.OrderCount
            );

            try
            {
                await RunAsyncFromWeb(orderProcessingOptions);
                return Ok("Processing started successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error starting processing: {ex.Message}");
            }
        }

        public static async Task RunAsyncFromWeb(OrderProcessingOptions orderProcessingOptions)
        {
            // 1. Connect to the SignalR hub
            var appSettings = Startup.GetAppSettings();

            var connection = new HubConnectionBuilder()
                .WithUrl(appSettings.SignalRHubUrl)
                .Build();

            await connection.StartAsync();

            // 2. Configure Serilog.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Sink(new SignalRSink(connection))
                .WriteTo.Console()
                .CreateLogger();

            // 3. Run the app.
            var serviceProvider = Startup.InitServiceProvider();

            var manageOrderService = serviceProvider.GetService<IProcessOrdersService>();
            var randomDishGeneratorService = serviceProvider.GetService<IRandomOrderGeneratorService>();

            Log.Information("[Main Thread] Getting orders...");
            var generatedOrders = randomDishGeneratorService.Generate(orderProcessingOptions);
            if (generatedOrders is null)
            {
                return;
            }

            Log.Information("[Main Thread] Successfully got orders. Starting to process the orders.");
            var orderStateChangeLogs = await manageOrderService.ProcessOrders(generatedOrders, orderProcessingOptions);
        }
    }
}