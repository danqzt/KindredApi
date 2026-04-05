using System.Text.Json;

namespace Common.Models;

public class BaseMessage
{
    public MessageType Type { get; set; }
    public JsonElement Payload { get; set; } // Parse the payload depending on the Type
    public DateTime Timestamp { get; set; }
}