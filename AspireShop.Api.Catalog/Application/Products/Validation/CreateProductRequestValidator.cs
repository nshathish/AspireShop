using AspireShop.Api.Catalog.Api.Contracts.Products;
using FluentValidation;

namespace AspireShop.Api.Catalog.Application.Products.Validation;

public sealed class CreateProductRequestValidator
    : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .PrecisionScale(18, 2, true);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ImageUrl)
            .MaximumLength(2048)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl),
                ApplyConditionTo.CurrentValidator)
            .WithMessage("ImageUrl must be a valid absolute URL.");
    }
}