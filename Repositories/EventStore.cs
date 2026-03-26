using KindredApi.Models.Events;

namespace KindredApi.Repositories;

public interface IEventStore
{
    void SaveEvent<T>(T @event);
    List<BetPlacedEvent> BetsPlacedByCustomer(int customerId);
}
public class EventStore : IEventStore
{
    private List<BetPlacedEvent> betsPlacedEvents = new();
    private List<FixtureEvent> fixtureEvents = new();
    
    public List<BetPlacedEvent> BetsPlacedByCustomer(int customerId)
    {
        var events = betsPlacedEvents.Where(e => customerId == e.CustomerId).OrderBy(s => s.Timestamp).ToList();
        return events.Any() ? events.ToList() : new List<BetPlacedEvent>();
    }
    
    public void SaveEvent<T>(T @event)
    {
        if (@event is BetPlacedEvent betsPlacedEvent)
        {
            betsPlacedEvents.Add(betsPlacedEvent);
        }

        if (@event is FixtureEvent fixtureEvent)
        {
            fixtureEvents.Add(fixtureEvent);
        }
    }
}