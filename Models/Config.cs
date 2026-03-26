namespace KindredApi.Models;

public record WageringServiceSettings
{
    public string Host { get; init; } = string.Empty;
    public string CandidateId { get; init; } = string.Empty;
}