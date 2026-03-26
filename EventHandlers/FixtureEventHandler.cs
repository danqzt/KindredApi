using System.Text.Json;
using KindredApi.Models.Events;
using KindredApi.Repositories;

namespace KindredApi.EventHandlers;

public class FixtureEventHandler(IEventStore eventStore)
{
    public async Task Handle(FixtureEvent @event, ILogger<FixtureEventHandler> logger)
    {
        try
        {
            eventStore.SaveEvent(@event);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                $"Failed to handle bet placed event: {JsonSerializer.Serialize(@event)} Error: ${ex.Message}");
            throw;
        }
    }
}