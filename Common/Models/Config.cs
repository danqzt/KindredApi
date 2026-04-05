namespace Common.Models;

public record WageringServiceSettings
{
    public string? Host { get; init; } = string.Empty;
    public string? CandidateId { get; init; } = string.Empty;
    
    public bool UseKafka { get; init; }
}