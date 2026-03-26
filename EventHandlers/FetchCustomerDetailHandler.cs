using KindredApi.Models.Commands;
using KindredApi.Repositories;

namespace KindredApi.EventHandlers;

public class FetchCustomerDetailHandler
{
    public async Task Handle(FetchCustomerDetail command, ICustomerClient client, ICustomerRepository repository, ILogger<FetchCustomerDetailHandler> logger)
    {
        try 
        {
            var customerDetail = await client.GetCustomerDetail(command.CustomerId);
            repository.SetCustomerName(customerDetail!.Id, customerDetail.Name);
        }
        catch (Exception ex)
        {
            // We "ignore" by logging and moving on, 
            logger.LogWarning(ex, "Failed to fetch details for customer {Id}, ignoring.", command.CustomerId);
        }
    }
}