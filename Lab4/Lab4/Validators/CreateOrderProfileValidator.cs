using FluentValidation;
using Lab4.Features.Orders.Requests;
using Lab4.Persistence.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Lab4.Validators;

public class CreateOrderProfileValidator : AbstractValidator<CreateOrderProfileRequest>
{
    public CreateOrderProfileValidator(IMemoryCache cache, ILogger<CreateOrderProfileValidator> logger)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(1).MaximumLength(200);
        
        RuleFor(x => x.Author)
            .NotEmpty().MinimumLength(2).MaximumLength(100)
            .Matches(@"^[a-zA-Z\s\-\.'’]+$").WithMessage("Invalid author name.");
        
        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN is required.")
            .Matches(@"^(?:\d{10}|\d{13})$").WithMessage("Invalid ISBN format (10 or 13 digits).")
            .MustAsync(async (isbn, _) =>
            {
                var orders = cache.Get<List<Order>>("all_orders") ?? new();
                bool exists = orders.Any(o => o.ISBN == isbn);
                logger.LogInformation("ISBN validation for {ISBN} - Exists? {Exists}", isbn, exists);
                return !exists;
            }).WithMessage("ISBN already exists.");
        
        RuleFor(x => new { x.Title, x.Author })
            .MustAsync(async (req, _) =>
            {
                var orders = cache.Get<List<Order>>("all_orders") ?? new();
                bool exists = orders.Any(o => o.Title == req.Title && o.Author == req.Author);
                logger.LogInformation("Title+Author validation - Exists? {Exists}", exists);
                return !exists;
            }).WithMessage("An order with the same title and author already exists.");
        
        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid category.");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.")
            .LessThan(10000).WithMessage("Price must be less than 10,000.");
        
        RuleFor(x => x.PublishedDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Cannot be in the future.")
            .GreaterThan(new DateTime(1400, 1, 1)).WithMessage("Date must be after year 1400.");

       
        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.")
            .LessThanOrEqualTo(100000).WithMessage("Stock too high.");

        
        ApplyBusinessRules();
    }
    
    private void ApplyBusinessRules()
    {
        When(x => x.Category == OrderCategory.Technical, () =>
        {
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(20).WithMessage("Technical orders must cost at least $20.");

            RuleFor(x => x.PublishedDate)
                .Must(date => date >= DateTime.UtcNow.AddYears(-5))
                .WithMessage("Technical orders must be recent (last 5 years).");
        });

        
        When(x => x.Category == OrderCategory.Children, () =>
        {
            RuleFor(x => x.Price)
                .LessThanOrEqualTo(50).WithMessage("Children's books cannot exceed $50.");

            RuleFor(x => x.Title)
                .Must(NotContainRestrictedWords)
                .WithMessage("Title contains inappropriate content for children.");
        });

        
        When(x => x.Category == OrderCategory.Fiction, () =>
        {
            RuleFor(x => x.Author.Length)
                .GreaterThanOrEqualTo(5).WithMessage("Fiction author name must be at least 5 characters.");
        });

        
        When(x => x.Price > 100, () =>
        {
            RuleFor(x => x.StockQuantity)
                .LessThanOrEqualTo(20).WithMessage("Expensive orders must have stock ≤ 20.");
        });
    }
    
    private bool NotContainRestrictedWords(string title)
    {
        var badWords = new[] { "violence", "blood", "adult" };
        return !badWords.Any(badWord => title.Contains(badWord, StringComparison.OrdinalIgnoreCase));
    }
}
