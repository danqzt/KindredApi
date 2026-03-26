using KindredApi.Models.Events;
using KindredApi.Repositories;
using Wolverine;

namespace KindredApi.EventHandlers;

public class BetsPlacedEventHandler(IEventStore eventStore, ICustomerRepository repository, ICustomerClient client)
{
    public async Task Handle(BetsPlacedEvent @event)
    {
       eventStore.SaveEvent(@event);
       var newCustomer = repository.BetPlaced(@event);
       if (newCustomer)
       {
           var custDetail = await client.GetCustomerDetail(@event.CustomerId);
           repository.SetCustomerName(custDetail.Id, custDetail.Name);
       }
    }
}