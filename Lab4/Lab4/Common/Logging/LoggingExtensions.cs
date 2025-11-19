using Microsoft.Extensions.Logging;
namespace Lab4.Common.Logging;
public static class LoggingExtensions
{
    public static void LogOrderCreationMetrics(
        this ILogger logger,
        OrderCreationMetrics metrics)
    {
        logger.LogInformation(
            LogEvents.OrderCreationCompleted,
            "Order creation completed - OperationId: {OperationId}, Title: {Title}, ISBN: {ISBN}, " +
            "Category: {Category}, Validation: {ValidationMs}ms, Database: {DatabaseMs}ms, " +
            "Total: {TotalMs}ms, Success: {Success}, Error: {Error}",
            metrics.OperationId,
            metrics.OrderTitle,
            metrics.ISBN,
            metrics.Category,
            metrics.ValidationDuration.TotalMilliseconds,
            metrics.DatabaseSaveDuration.TotalMilliseconds,
            metrics.TotalDuration.TotalMilliseconds,
            metrics.Success,
            metrics.ErrorReason ?? "None");
    }
}