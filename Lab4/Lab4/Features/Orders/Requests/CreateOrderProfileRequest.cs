using Lab4.Persistence.Domain;

namespace Lab4.Features.Orders.Requests;

public class CreateOrderProfileRequest
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public OrderCategory Category { get; set; }
    public decimal Price { get; set; }
    public DateTime PublishedDate { get; set; }
    public string? CoverImageUrl { get; set; }
    public int StockQuantity { get; set; } = 1;
}