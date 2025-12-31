using System.Text.Json.Serialization;

namespace CSS.Challenge.Domain.Models;

/// <summary>
/// OrderStateChangeLog is a json-friendly representation of an action.
/// </summary>
/// <param name="timestamp">action timestamp</param>
/// <param name="id">order id</param>
/// <param name="action">place, move, pickup or discard</param>
/// <param name="target">heater, cooler or shelf. Target is the destination for move</param>
public class OrderStateChangeLog(DateTime timestamp, string id, string action, string target) {
    public static readonly string Place = "place";
    public static readonly string Move = "move";
    public static readonly string Pickup = "pickup";
    public static readonly string Discard = "discard";

    public static readonly string Heater = "heater";
    public static readonly string Cooler = "cooler";
    public static readonly string Shelf = "shelf";

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; } = (long)timestamp.Subtract(DateTime.UnixEpoch).TotalMicroseconds;
    [JsonPropertyName("id")]
    public string Id { get; init; } = id;
    [JsonPropertyName("action")]
    public string Action_ { get; init; } = action;
    [JsonPropertyName("target")]
    public string Target { get; init; } = target;
};