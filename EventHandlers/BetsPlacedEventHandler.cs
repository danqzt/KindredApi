using System.Text.Json;
using KindredApi.Models.Commands;
using KindredApi.Models.Events;
using KindredApi.Repositories;
using Wolverine;

namespace KindredApi.EventHandlers;

public class BetPlacedEventHandler(IEventStore eventStore, IMessageBus messageBus, ICustomerRepository repository, ILogger<BetPlacedEventHandler> logger)
{
    public async Task Handle(BetPlacedEvent @event)
    {
        try
        {
            eventStore.SaveEvent(@event);
            var newCustomer = repository.BetPlaced(@event);
            if (newCustomer)
            {
                logger.LogInformation($"New customer: {@event.CustomerId}");
                await messageBus.PublishAsync(new FetchCustomerDetail(@event.CustomerId));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to handle bet placed event: {JsonSerializer.Serialize(@event)} Error: ${ex.Message}");
            throw;
        }
    }
}