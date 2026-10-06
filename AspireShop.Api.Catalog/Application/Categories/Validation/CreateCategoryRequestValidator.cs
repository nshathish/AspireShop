using AspireShop.Api.Catalog.Api.Contracts.Categories;
using FluentValidation;

namespace AspireShop.Api.Catalog.Application.Categories.Validation;

public sealed class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(category => category.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(category => category.Slug)
            .NotEmpty()
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .MaximumLength(120);
    }
}
