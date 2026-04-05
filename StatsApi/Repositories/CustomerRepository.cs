using System.Collections.Concurrent;
using KindredApi.Models.Domain;
using KindredApi.Models.Events;

namespace KindredApi.Repositories;

public interface ICustomerRepository
{
    CustomerAggregate? GetCustomer(int customerId);
    bool BetPlaced(BetPlacedEvent @event);
    public void SetCustomerName(int customerId, string name);
}
public class CustomerRepository : ICustomerRepository
{
    private ConcurrentDictionary<int, CustomerAggregate> _customers = new();

    public CustomerAggregate? GetCustomer(int customerId)
    {
        _customers.TryGetValue(customerId, out var customer);
        return customer;
    }

    public bool BetPlaced(BetPlacedEvent @event)
    {
        var customer = GetCustomer(@event.CustomerId);
        if (customer == null)
        {
            _customers.GetOrAdd(@event.CustomerId, CustomerAggregate.New(@event));
            return true;
        }
        customer.Apply(@event);
        return false;
    }
    
    public void SetCustomerName(int customerId, string name)
    {
        var customer = GetCustomer(customerId);
        if (customer != null)
        {
            customer.CustomerName = name;
        }
    }
}