using Xunit;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Lab4.Features.Orders.Handlers;
using Lab4.Features.Orders.Requests;
using Lab4.Persistence.Domain;
using Lab4.Common.Mappings;

namespace Lab4.Tests.Handlers;

public class CreateOrderHandlerIntegrationTests
{
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerIntegrationTests()
    {
        var logger = LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger<CreateOrderHandler>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AdvancedOrderMappingProfile>();
        });
        var mapper = mapperConfig.CreateMapper();

        _handler = new CreateOrderHandler(logger, memoryCache, mapper);
    }

    [Fact] 
    public async Task Should_Create_Order_Successfully()
    {
        var request = new CreateOrderProfileRequest
        {
            Title = "Clean Code",
            Author = "Robert Martin",
            ISBN = "1234567890",
            Category = OrderCategory.Technical,
            Price = 45,
            PublishedDate = DateTime.UtcNow.AddYears(-1),
            StockQuantity = 10
        };

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.ISBN, result.ISBN);
        Assert.Equal("Technical & Professional", result.CategoryDisplayName);
    }
}