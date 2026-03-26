using KindredApi.Models;
using KindredApi.Repositories;

namespace KindredApi.Services;

public interface ICustomerService
{
    Task<CustomerDto?> GetCustomerStat(int customerId);
}
public class CustomerService(ICustomerRepository customerRepository, ICustomerClient client) : ICustomerService
{
    public async Task<CustomerDto?> GetCustomerStat(int customerId)
    {
        var customer = customerRepository.GetCustomer(customerId);
        if (customer == null)
        {
            return null;
        }
        if (string.IsNullOrWhiteSpace(customer.CustomerName))
        {
           var detail = await client.GetCustomerDetail(customerId);
           if (detail == null)
           {
               //throw exception when cannot find customer detail
               throw new KeyNotFoundException($"Customer detail not found for ID {customerId}");
           }

           customerRepository.SetCustomerName(customerId, detail.Name);
        }
        
        return new CustomerDto
        { 
            CustomerId = customer.CustomerId,
            Name = customer.CustomerName,
            TotalStandToWin = customer.TotalStandToWin
        };
    }
}