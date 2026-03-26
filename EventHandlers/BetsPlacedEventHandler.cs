using KindredApi.Models.Commands;
using KindredApi.Models.Events;
using KindredApi.Repositories;
using Wolverine;

namespace KindredApi.EventHandlers;

public class BetPlacedEventHandler(IEventStore eventStore, IMessageBus messageBus, ICustomerRepository repository)
{
    public async Task Handle(BetPlacedEvent @event)
    {
       eventStore.SaveEvent(@event);
       var newCustomer = repository.BetPlaced(@event);
       if (newCustomer)
       {
           await messageBus.PublishAsync(new FetchCustomerDetail(@event.CustomerId));
       }
    }
}