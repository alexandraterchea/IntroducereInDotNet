using System.ComponentModel.DataAnnotations;

namespace Lab4.Validators.Attributes;

public class ValidISBNAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string isbn)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(isbn, @"^(?:\d{10}|\d{13})$"))
                return ValidationResult.Success;
        }
        return new ValidationResult("Invalid ISBN format");
    }
}