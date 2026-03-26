using KindredApi.Models.Events;
using KindredApi.Repositories;

namespace KindredApi.EventHandlers;

public class FixtureEventHandler(IEventStore eventStore)
{
    public async Task Handle(FixtureEvent @event)
    { 
        eventStore.SaveEvent(@event);
    }
}