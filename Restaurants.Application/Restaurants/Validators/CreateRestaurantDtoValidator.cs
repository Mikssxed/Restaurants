using FluentValidation;
using Restaurants.Application.Restaurants.Dtos;

namespace Restaurants.Application.Restaurants.Validators;

public class CreateRestaurantDtoValidator : AbstractValidator<CreateRestaurantDto>
{
    private readonly List<string> allowedCategories = new()
    {
        "Italian",
        "Chinese",
        "Indian",
        "Mexican",
        "American",
        "Thai",
        "French",
        "Japanese"
    };
    public CreateRestaurantDtoValidator()
    {
        RuleFor(x => x.Name).Length(3, 100).NotEmpty();
        RuleFor(x => x.Category).NotEmpty().Must(allowedCategories.Contains);
        RuleFor(x => x.ContactEmail).EmailAddress();
        RuleFor(x => x.ContactNumber).Length(3, 12).NotEmpty();
        RuleFor(x => x.PostalCode).Matches(@"^\d{2}-\d{3}$");
    }
}