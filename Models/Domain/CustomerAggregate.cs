using KindredApi.Models.Events;

namespace KindredApi.Models.Domain;

public class CustomerAggregate
{
    public int CustomerId { get; init; }
    
    public string CustomerName { get; set; }
    public double TotalPayout { get; private set; }
    public double TotalStake { get; private set; }
    public double TotalStandToWin { get; private set; }

    public void Apply(BetPlacedEvent @event)
    {
        TotalStake += @event.Stake;
        TotalPayout += @event.Stake * @event.Odds;
        TotalStandToWin += @event.Stake * @event.Odds - @event.Stake;
    }
    
    public static CustomerAggregate New(BetPlacedEvent @event)
    {
        var newCustomer = new CustomerAggregate
        {
            CustomerId = @event.CustomerId,
        };
        newCustomer.Apply(@event);
        return newCustomer;
    }
}