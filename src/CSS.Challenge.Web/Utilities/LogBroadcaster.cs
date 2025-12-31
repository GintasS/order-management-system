using CSS.Challenge.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CSS.Challenge.Web.Utilities
{
    public class LogBroadcaster
    {
        private readonly IHubContext<LogHub> _hubContext;

        public LogBroadcaster(IHubContext<LogHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastLog(string logType, string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveLog", logType, message);
        }
    }
}