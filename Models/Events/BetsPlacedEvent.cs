namespace KindredApi.Models.Events;

public record BetsPlacedEvent: BaseEvent
{
    public int FixtureId { get; init; }
    public required string OutcomeKey { get; init; }
    public int CustomerId { get; init; }
    public double Odds { get; init; }
    public double Stake { get; init; }
};