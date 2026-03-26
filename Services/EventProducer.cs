using System.Text.Json;
using KindredApi.Models.Events;
using KindredApi.Models.External;
using Wolverine;
using Wolverine.Runtime;

namespace KindredApi.Services;

public interface IEventProducer
{
    Task PublishAsync(BaseMessage message);
}

/// <summary>
/// Need to use busRuntime to be injected to singleton hostedservice
/// </summary>
/// <param name="busRuntime"></param>
public class EventProducer(IWolverineRuntime busRuntime) : IEventProducer
{
    public async Task PublishAsync(BaseMessage message)
    {
        dynamic @event;
        switch (message.Type)
        {
            case MessageType.Fixture:
                @event = message.Payload.Deserialize<FixtureEvent>()!;
                break;
            case MessageType.BetPlaced:
                @event = message.Payload.Deserialize<BetPlacedEvent>()!;
                break;
            default:
                throw new InvalidOperationException($"Unsupported message type: {message.Type}");
        }

        @event.Timestamp = message.Timestamp;
        var messageBus = new MessageBus(busRuntime);
        await messageBus.SendAsync(@event); 
    }
}