using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Lab4.Persistence.Domain;
using Lab4.Features.Orders.Requests;
using Lab4.Features.Orders.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Lab4.Common.Logging;

namespace Lab4.Features.Orders.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderProfileRequest, OrderProfileDto>
{
    private readonly ILogger<CreateOrderHandler> _logger;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;
    
    private const string CacheKey = "all_orders";
    
    public CreateOrderHandler(ILogger<CreateOrderHandler> logger, IMemoryCache cache, IMapper mapper)
    {
        _logger = logger;
        _cache = cache;
        _mapper = mapper;
    }

    public async Task<OrderProfileDto> Handle(CreateOrderProfileRequest request, CancellationToken cancellationToken)
    {
        var operationId = Guid.NewGuid().ToString()[..8];
        var operationStart = DateTime.UtcNow;
        TimeSpan validationDuration = TimeSpan.Zero;
        TimeSpan databaseDuration = TimeSpan.Zero;

        using (_logger.BeginScope(new Dictionary<string, object> { ["OperationId"] = operationId }))
        {
            try
            {
                _logger.LogInformation(
                    LogEvents.OrderCreationStarted,
                    "Starting order creation - OperationId: {OperationId}, Title: {Title}, Author: {Author}, ISBN: {ISBN}, Category: {Category}",
                    operationId, request.Title, request.Author, request.ISBN, request.Category);

                // Validation phase
                var validationStart = DateTime.UtcNow;
                var orders = _cache.Get<List<Order>>(CacheKey) ?? new List<Order>();
                
                _logger.LogInformation(
                    LogEvents.ISBNValidationPerformed,
                    "Performing ISBN validation for {ISBN}",
                    request.ISBN);

                if (orders.Any(o => string.Equals(o.ISBN, request.ISBN, StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogWarning(
                        LogEvents.OrderValidationFailed,
                        "Duplicate ISBN detected: {ISBN}",
                        request.ISBN);
                    throw new InvalidOperationException($"An order with ISBN {request.ISBN} already exists.");
                }

                _logger.LogInformation(
                    LogEvents.StockValidationPerformed,
                    "Stock quantity validated: {StockQuantity} for ISBN: {ISBN}",
                    request.StockQuantity, request.ISBN);

                validationDuration = DateTime.UtcNow - validationStart;

                // Database operation phase
                var dbStart = DateTime.UtcNow;
                _logger.LogInformation(
                    LogEvents.DatabaseOperationStarted,
                    "Starting database save operation for ISBN: {ISBN}",
                    request.ISBN);

                var order = _mapper.Map<Order>(request);
                orders.Add(order);

                _logger.LogInformation(
                    LogEvents.CacheOperationPerformed,
                    "Updating cache with key: {CacheKey}, Total orders: {OrderCount}",
                    CacheKey, orders.Count);

                _cache.Set(CacheKey, orders);
                
                _logger.LogInformation(
                    LogEvents.DatabaseOperationCompleted,
                    "Database operation completed - OrderId: {OrderId}, ISBN: {ISBN}",
                    order.Id, order.ISBN);

                databaseDuration = DateTime.UtcNow - dbStart;

                var dto = _mapper.Map<OrderProfileDto>(order);
                var totalDuration = DateTime.UtcNow - operationStart;

                // Log comprehensive metrics
                _logger.LogOrderCreationMetrics(new OrderCreationMetrics
                {
                    OperationId = operationId,
                    OrderTitle = request.Title,
                    ISBN = request.ISBN,
                    Category = request.Category,
                    ValidationDuration = validationDuration,
                    DatabaseSaveDuration = databaseDuration,
                    TotalDuration = totalDuration,
                    Success = true,
                    ErrorReason = null
                });

                _logger.LogInformation(
                    "Order created successfully - ISBN: {ISBN}, Title: {Title}, Total time: {TotalMs}ms",
                    request.ISBN, request.Title, totalDuration.TotalMilliseconds);

                return await Task.FromResult(dto);
            }
            catch (Exception ex)
            {
                var totalDuration = DateTime.UtcNow - operationStart;
                
                
                _logger.LogOrderCreationMetrics(new OrderCreationMetrics
                {
                    OperationId = operationId,
                    OrderTitle = request.Title,
                    ISBN = request.ISBN,
                    Category = request.Category,
                    ValidationDuration = validationDuration,
                    DatabaseSaveDuration = databaseDuration,
                    TotalDuration = totalDuration,
                    Success = false,
                    ErrorReason = ex.Message
                });

                _logger.LogError(
                    ex,
                    "Error while creating order - OperationId: {OperationId}, ISBN: {ISBN}, Error: {ErrorMessage}",
                    operationId, request.ISBN, ex.Message);
                
                throw;
            }
        }
    }
}