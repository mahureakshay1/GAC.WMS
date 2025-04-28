using FluentValidation;
using GAC.WMS.Application.Dtos;

namespace GAC.WMS.Application.Validators
{
    public class PurchaseOrderLineDtoValidator : AbstractValidator<PurchaseOrderLineDto>
    {
        public PurchaseOrderLineDtoValidator()
        {
            RuleFor(x => x.PurchaseOrderId)
                .NotEmpty()
                .WithMessage("Purchase order ID is required.");
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required.");
            RuleFor(x => x.Quantity)
                .NotEmpty()
                .WithMessage("Quantity is required.")
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");
            RuleFor(x => x.UnitPrice)
                .NotEmpty()
                .WithMessage("Unit price is required.")
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than 0.");
            RuleFor(x => x.TotalPrice)
                .NotEmpty()
                .WithMessage("Total price is required.")
                .GreaterThan(0)
                .WithMessage("Total price must be greater than 0.")
                .Equal(x => x.Quantity * x.UnitPrice)
                .WithMessage("Total price must be equal to Quantity * Unit Price.");
        }
    }
}
