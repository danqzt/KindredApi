using System.Text.Json;
using KindredApi.Models;
using KindredApi.Models.External;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace KindredApi.Repositories;

public interface ICustomerClient
{
    Task<CustomerDetailDto?> GetCustomerDetail(int customerId);
}
public class CustomerClient(HttpClient client, IMemoryCache cache, IOptions<WageringServiceSettings> options, ILogger<CustomerClient> logger) : ICustomerClient
{
    WageringServiceSettings Settings => options.Value;
    string CacheKey(int customerId) => $"customer_{customerId}";

    public async Task<CustomerDetailDto?> GetCustomerDetail(int customerId)
    {
        cache.TryGetValue(CacheKey(customerId), out CustomerDetailDto customerDetail);
        if (customerDetail == null)
        {
            try
            {
                var response =
                    await client.GetAsync($"customer?customerId={customerId}&candidateId={Settings.CandidateId}");

                response.EnsureSuccessStatusCode();
                var stringValue = await response.Content.ReadAsStringAsync();
                customerDetail = JsonSerializer.Deserialize<CustomerDetailDto>(stringValue)!;
                cache.Set(CacheKey(customerId), customerDetail);
            }
            catch(Exception e)
            {
                logger.LogError($"Error calling customer client: {e.Message}");
            }
        }
        return customerDetail;
    }
}