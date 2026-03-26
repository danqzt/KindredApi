using KindredApi.Models;
using KindredApi.Repositories;

namespace KindredApi.Services;

public interface ICustomerService
{
    Task<CustomerDto> GetCustomerStat(int customerId);
}
public class CustomerService(ICustomerRepository customerRepository, ICustomerClient client) : ICustomerService
{
    public async Task<CustomerDto> GetCustomerStat(int customerId)
    {
        var customer = customerRepository.GetCustomer(customerId);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
        }
        if (string.IsNullOrWhiteSpace(customer.CustomerName))
        {
           var detail = await client.GetCustomerDetail(customerId);
           if (detail == null)
           {
               throw new KeyNotFoundException($"Customer detail not found for ID {customerId}");
           }

           customer.CustomerName = detail.Name;
        }
        
        return new CustomerDto
        { 
            CustomerId = customer.CustomerId,
            Name = customer.CustomerName,
            TotalStandToWin = customer.TotalStandToWin
        };
    }
}