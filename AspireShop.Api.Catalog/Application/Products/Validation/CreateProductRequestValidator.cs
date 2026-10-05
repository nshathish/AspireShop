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
            .GreaterThan(0);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0);
    }
}