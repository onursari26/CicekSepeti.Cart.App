using CicekSepeti.Api.Models;
using FluentValidation;

namespace CicekSepeti.Api.Validations
{
    public class BasketValidator : AbstractValidator<BasketItemViewModel>
    {
        public BasketValidator()
        {
            RuleFor(c => c.ProductId).NotNull().NotEmpty().NotEqual(0).WithMessage(ValidationMessage.NotEmpty);

            RuleFor(c => c.CurrentQuantity).NotNull().NotEmpty().NotEqual(0).WithMessage(ValidationMessage.NotEmpty);
        }
    }

    public class DiscountValidator : AbstractValidator<DiscountViewModel>
    {
        public DiscountValidator()
        {
            RuleFor(c => c.Code).NotNull().NotEmpty().WithMessage(ValidationMessage.NotEmpty);
        }
    }
}
