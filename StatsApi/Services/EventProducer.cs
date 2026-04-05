using System.Text.Json;
using KindredApi.Models.Events;
using Common.Models;
using Wolverine;

namespace KindredApi.Services;

public interface IEventProducer
{
    Task PublishAsync(BaseMessage message);
}


public class EventProducer(IServiceScopeFactory scopeFactory, ILogger<EventProducer> logger) : IEventProducer
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
                logger.LogWarning($"Unsupported message type: {message.Type}");
                return;
        }

        @event.Timestamp = message.Timestamp;
        
        //create temporary singleton scope to get message bus
        using var scope = scopeFactory.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        await messageBus.PublishAsync(@event); 
    }
}