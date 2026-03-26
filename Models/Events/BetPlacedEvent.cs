using System.Data;
using FluentValidation;

namespace KindredApi.Models.Events;

public record BetPlacedEvent: BaseEvent
{
    public int FixtureId { get; init; }
    public required string OutcomeKey { get; init; }
    public int CustomerId { get; init; }
    public double Odds { get; init; }
    public double Stake { get; init; }
};

public class BetPlacedEventValidator: AbstractValidator<BetPlacedEvent>
{
    public BetPlacedEventValidator()
    {
        RuleFor(e => e.CustomerId).GreaterThan(0);
        RuleFor(e => e.OutcomeKey).NotNull().NotEmpty();
        RuleFor(e => e.FixtureId).GreaterThan(0);
        RuleFor(e => e.Odds).GreaterThan(1.0);
        RuleFor(e => e.Stake).GreaterThan(0);
    }
}
