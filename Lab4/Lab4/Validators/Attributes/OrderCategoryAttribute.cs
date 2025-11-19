using System.ComponentModel.DataAnnotations;
using Lab4.Persistence.Domain;

namespace Lab4.Validators.Attributes;

public class OrderCategoryAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (Enum.IsDefined(typeof(OrderCategory), value))
            return ValidationResult.Success;

        return new ValidationResult($"{value} is not a valid order category.");
    }
}