using CSS.Challenge.Domain.Interfaces;
using CSS.Challenge.Domain.Models;
using CSS.Challenge.Domain.Utilities;
using CSS.Challenge.Domain.Configuration;
using CSS.Challenge.Domain.Threads;
using Serilog;
using CSS.Challenge.Domain.States;
using static CSS.Challenge.Domain.States.OrderStates;

namespace CSS.Challenge.Domain.Services
{
    /// <summary>
    /// Main Service class that handles all states of the order
    /// and processes every order using threads.
    /// </summary>
    public class ProcessOrdersService : IProcessOrdersService
    {
        public static bool AreThreadsStarted { get; private set; }

        private readonly OrderProcessingOptions _options;
        private readonly Thread _threadFreshness;
        private readonly Thread _threadRemovePickupTime;
        private readonly Thread _threadPrintSizes;

        public ProcessOrdersService()
        {
            _threadFreshness = new Thread(ThreadFreshness.ThreadRemoveFreshness);
            _threadRemovePickupTime = new Thread(ThreadPickupTime.ThreadRemovePickupTime);
            _threadPrintSizes = new Thread(ThreadListSizes.ThreadPrintListSizes);

        }

        /// <summary>
        /// Processes all orders.
        /// </summary>
        /// <param name="response">API Order response.</param>
        /// <returns>A list of OrderStateChangeLog that will be sent to API.</returns>
        public async Task<List<OrderStateChangeLog>> ProcessOrders(List<OrderModel> orders, OrderProcessingOptions options)
        {
            PreprocessSingleOrderStateInstance.SetProcessingOptions(options);
            ConcurrentDataStructures.InitializeCapacities(options);
            AreThreadsStarted = false;

            try
            {
                foreach (var order in orders)
                {

                    var context = new Context(PreprocessSingleOrderStateInstance);
                    context.ExecuteState(order);

                    if (AreThreadsStarted is false)
                    {
                        StartThreads();
                    }
                    await Task.Delay(options.Rate);
                }
            }
            catch (Exception e)
            {
                Log.Error($"[Main Thread]: Order processing has failed: {e}"); ;
            }

            Log.Information($"[Main Thread]: Blocking the main thread to wait for background threads to finish.");
            _threadFreshness.Join();
            _threadRemovePickupTime.Join();
            _threadPrintSizes.Join();

            Log.Information($"[Main Thread]: All external threads have finished, returning results.");

            return OrderStateChangeLogHelper.AllActions.ToList();
        }


        private void StartThreads()
        {
            Log.Information($"[Main Thread]: Starting background threads for freshness removal, pickup time removal, and size printing.");
            _threadFreshness.Start();
            _threadRemovePickupTime.Start();
            _threadPrintSizes.Start();

            AreThreadsStarted = true;
        }
    }
}
