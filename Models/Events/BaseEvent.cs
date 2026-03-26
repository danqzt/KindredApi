namespace KindredApi.Models.Events;

public record BaseEvent
{
    public DateTime Timestamp { get; set; }
}