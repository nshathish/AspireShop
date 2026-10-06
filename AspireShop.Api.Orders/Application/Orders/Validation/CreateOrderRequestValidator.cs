using AspireShop.Api.Orders.Api.Contracts.Orders;
using FluentValidation;

namespace AspireShop.Api.Orders.Application.Orders.Validation;

public sealed class CreateOrderRequestValidator
    : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(order => order.CustomerName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(order => order.CustomerEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(order => order.Total)
            .GreaterThan(0)
            .PrecisionScale(18, 2, true);
    }
}
