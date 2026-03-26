using System.Text.Json.Serialization;

namespace KindredApi.Models.External;

public class CustomerDetailDto
{
    public int Id { get; set; }

    [JsonPropertyName("customerName")] 
    public string Name { get; set; } = null!;
}