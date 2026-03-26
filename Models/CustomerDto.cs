namespace KindredApi.Models;

public record CustomerDto 
{
    public int CustomerId { get; init; }
    public string? Name { get; set; }
    public decimal TotalStandToWin { get; set; }
}