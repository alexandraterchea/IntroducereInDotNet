using Lab4.Persistence.Domain;
namespace Lab4.Common.Logging;
public record OrderCreationMetrics
{
    public string OperationId { get; init; }
    public string OrderTitle { get; init; }
    public string ISBN { get; init; }
    public OrderCategory Category { get; init; }
    public TimeSpan ValidationDuration { get; init; }
    public TimeSpan DatabaseSaveDuration { get; init; }
    public TimeSpan TotalDuration { get; init; }
    public bool Success { get; init; }
    public string? ErrorReason { get; init; }
}