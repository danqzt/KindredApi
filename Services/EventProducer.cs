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


public class EventProducer(IServiceScopeFactory scopeFactory) : IEventProducer
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
        
        //create temporary singleton scope to get message bus
        using var scope = scopeFactory.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        await messageBus.PublishAsync(@event); 
    }
}