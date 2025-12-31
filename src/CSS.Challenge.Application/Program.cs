using CSS.Challenge.Application.Enums;
using CSS.Challenge.Domain;
using CSS.Challenge.Domain.Configuration;
using CSS.Challenge.Domain.Interfaces;
using CSS.Challenge.Domain.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace CSS.Challenge.Application
{
    public class Program
    {
        static async Task<int> Main()
        {
            // 3. Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            // 4. Run the app.
            var orderProcessingOptions = new OrderProcessingOptions(500, 6, 6, 6, 4, 8, 40, 80, 10);

            var serviceProvider = Startup.InitServiceProvider();

            var manageOrderService = serviceProvider.GetService<IProcessOrdersService>();
            var randomDishGeneratorService = serviceProvider.GetService<IRandomOrderGeneratorService>();

            Log.Information("[Main Thread] Getting orders...");
            var generatedOrders = randomDishGeneratorService.Generate(orderProcessingOptions);
            if (generatedOrders is null)
            {
                return (int)ExitCode.ErrorOnGetOrdersEmptyResponse;
            }

            Log.Information("[Main Thread] Successfully got orders. Starting to process the orders.");
            var orderStateChangeLogs = await manageOrderService.ProcessOrders(generatedOrders, orderProcessingOptions);
            if (orderStateChangeLogs is null)
            {
                return (int)ExitCode.ErrorProcessingOrders;
            }

            return (int)ExitCode.Success;
        }
    }
}
