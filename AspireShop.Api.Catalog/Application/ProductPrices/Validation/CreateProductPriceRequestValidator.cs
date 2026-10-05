using AspireShop.Api.Catalog.Api.Contracts.ProductPrices;
using FluentValidation;

namespace AspireShop.Api.Catalog.Application.ProductPrices.Validation;

public sealed class CreateProductPriceRequestValidator : AbstractValidator<CreateProductPriceRequest>
{
    public CreateProductPriceRequestValidator()
    {
        RuleFor(price => price.ProductId).NotEmpty();
        RuleFor(price => price.Amount).GreaterThan(0);
        RuleFor(price => price.Currency)
            .NotEmpty()
            .Length(3)
            .Matches("^[A-Za-z]{3}$");
        RuleFor(price => price.ValidTo)
            .GreaterThan(price => price.ValidFrom)
            .When(price => price.ValidTo.HasValue);
    }
}
