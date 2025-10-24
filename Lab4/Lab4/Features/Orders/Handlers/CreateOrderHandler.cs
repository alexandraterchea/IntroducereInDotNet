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

namespace Lab4.Features.Orders.Handlers;

public class CreateOrderHandler:IRequestHandler<CreateOrderProfileRequest,OrderProfileDto>
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
        try
        {
            _logger.LogInformation(
                "Creating order: Title={Title}, Author={Author}, Category={Category}, ISBN={ISBN}",
                request.Title, request.Author, request.Category, request.ISBN);

            var orders = _cache.Get<List<Order>>(CacheKey) ?? new List<Order>();
            
            if (orders.Any(o =>string.Equals( o.ISBN, request.ISBN, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Duplicate ISBN detected: {ISBN}", request.ISBN);
                throw new InvalidOperationException($"An order with ISBN {request.ISBN} already exists.");
            }
            
            var order = _mapper.Map<Order>(request);
            orders.Add(order);
            _cache.Set(CacheKey, orders);
            
            var dto = _mapper.Map<OrderProfileDto>(order);
            _logger.LogInformation("Order created successfully for ISBN {ISBN}", request.ISBN);

            return await Task.FromResult(dto);
        }
        catch ( Exception ex)
        {
            _logger.LogError(ex,"Error while creating order with ISBN {ISBN}", request.ISBN);
            throw;
        }
    }
}