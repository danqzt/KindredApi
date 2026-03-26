using KindredApi.Models.Events;

namespace KindredApi.Repositories;

public interface IEventStore
{
    void SaveEvent<T>(T @event);
    List<BetsPlacedEvent> BetsPlacedByCustomer(int customerId);
}
public class EventStore : IEventStore
{
    private List<BetsPlacedEvent> betsPlacedEvents = new List<BetsPlacedEvent>();
    private List<FixtureEvent> fixtureEvents = new List<FixtureEvent>();
    
    public List<BetsPlacedEvent> BetsPlacedByCustomer(int customerId)
    {
        var events = betsPlacedEvents.Where(e => customerId == e.CustomerId).OrderBy(s => s.Timestamp).ToList();
        return events.Any() ? events.ToList() : new List<BetsPlacedEvent>();
    }
    
    public void SaveEvent<T>(T @event)
    {
        if (@event is BetsPlacedEvent betsPlacedEvent)
        {
            betsPlacedEvents.Add(betsPlacedEvent);
        }

        if (@event is FixtureEvent fixtureEvent)
        {
            fixtureEvents.Add(fixtureEvent);
        }
    }
}