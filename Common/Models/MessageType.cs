using System.Text.Json.Serialization;

namespace Common.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MessageType
{
    Fixture, BetPlaced, EndofFeed
}