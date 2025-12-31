using Microsoft.AspNetCore.SignalR.Client;
using Serilog.Core;
using Serilog.Events;

public class SignalRSink : ILogEventSink
{
    private readonly HubConnection _connection;

    public SignalRSink(HubConnection connection)
    {
        _connection = connection;
    }

    public void Emit(LogEvent logEvent)
    {
        var logLevel = logEvent.Level.ToString().ToLower();
        var message = logEvent.RenderMessage();

        // Send the log to the SignalR hub
        _ = _connection.InvokeAsync("BroadcastLog", logLevel, message);
    }
}