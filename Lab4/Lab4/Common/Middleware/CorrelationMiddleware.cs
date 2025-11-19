public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault() 
                            ?? Guid.NewGuid().ToString();
        
        context.Response.Headers[CorrelationIdHeader] = correlationId;
        
        using (var scope = context.RequestServices.GetRequiredService<ILoggerFactory>()
                   .CreateLogger<CorrelationMiddleware>()
                   .BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await _next(context);
        }
    }
}