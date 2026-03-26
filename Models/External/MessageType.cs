using System.Text.Json.Serialization;

namespace KindredApi.Models.External;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MessageType
{
    Fixture, BetPlaced, EndofFeed
}