namespace KindredApi.Models.Events;

public record FixtureEvent : BaseEvent
{
    public int Id {get; init;}
    public DateTime StartTime {get; init;}
    public required string Name { get; init; }
    public Outcome[] Outcomes {get; init;} = Array.Empty<Outcome>();
}

public record Outcome
{
    public required string Key { get; init; }
    public required string Name { get; init; }
}