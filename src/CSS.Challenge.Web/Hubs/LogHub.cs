using Microsoft.AspNetCore.SignalR;

namespace CSS.Challenge.Web.Hubs
{
    public class LogHub : Hub
    {
        public async Task BroadcastLog(string logType, string message)
        {
            await Clients.All.SendAsync("ReceiveLog", logType, message);
        }
    }
}