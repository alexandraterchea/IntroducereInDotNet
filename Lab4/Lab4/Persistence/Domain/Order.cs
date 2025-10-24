using System;

namespace Lab4.Persistence.Domain;

public class Order
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public OrderCategory Category { get; set; }
    public decimal Price { get; set; }
    public DateTime PublishedDate { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}