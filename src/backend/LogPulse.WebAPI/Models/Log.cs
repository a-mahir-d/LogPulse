namespace LogPulse.WebAPI.Models;

public record Log
{
    public int Id { get; init; }

    public required LogLevel Level { get; init; }

    public required string Message { get; init; }

    public required string SourceService { get; init; }

    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
