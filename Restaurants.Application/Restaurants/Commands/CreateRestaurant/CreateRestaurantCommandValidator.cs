using FluentValidation;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
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
    public CreateRestaurantCommandValidator()
    {
        RuleFor(x => x.Name).Length(3, 100).NotEmpty();
        RuleFor(x => x.Category).NotEmpty().Must(allowedCategories.Contains);
        RuleFor(x => x.ContactEmail).EmailAddress();
        RuleFor(x => x.ContactNumber).Length(3, 12).NotEmpty();
        RuleFor(x => x.PostalCode).Matches(@"^\d{2}-\d{3}$");
    }
}