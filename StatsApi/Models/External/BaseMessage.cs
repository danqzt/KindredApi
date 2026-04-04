using System.Text.Json;

namespace KindredApi.Models.External;

public class BaseMessage
{
    public MessageType Type { get; set; }
    public JsonElement Payload { get; set; } // Parse the payload depending on the Type
    public DateTime Timestamp { get; set; }
}


