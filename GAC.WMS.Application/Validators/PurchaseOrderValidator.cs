using FluentValidation;
using GAC.WMS.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAC.WMS.Application.Validators
{
    public class PurchaseOrderValidator : AbstractValidator<PurchaseOrderDto>
    {
        public PurchaseOrderValidator()
        {
            RuleFor(x => x.ProcessingDate)
                .NotEmpty()
                .WithMessage("Processing date cannot be empty.")
                .LessThan(DateTime.UtcNow)
                .WithMessage("Processing date must be in the past.");

            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer Id cannot be empty.");

           RuleFor(x=>x.Status)
                .NotEmpty()
                .WithMessage("Status cannot be empty.")
                .IsInEnum()
                .WithMessage("Status must be a valid enum value.");
            RuleFor(x => x.CreatedAt)
                .NotEmpty()
                .WithMessage("Created at cannot be empty.")
                .LessThan(DateTime.UtcNow)
                .WithMessage("Created at must be in the past.");
            RuleFor(x => x.PurchaseOrderLines)
                .NotEmpty()
                .WithMessage("Purchase order lines cannot be empty.")
                .Must(lines => lines.Any())
                .WithMessage("At least one purchase order line is required.");
            RuleForEach(x => x.PurchaseOrderLines)
                .ChildRules(lines =>
                {
                    lines.RuleFor(x => x.ProductId)
                        .NotEmpty()
                        .WithMessage("Product Id cannot be empty.");
                    lines.RuleFor(x => x.Quantity)
                        .NotEmpty()
                        .WithMessage("Quantity cannot be empty.")
                        .GreaterThan(0)
                        .WithMessage("Quantity must be greater than 0.");
                    lines.RuleFor(x => x.UnitPrice)     
                        .NotEmpty()
                        .WithMessage("Unit price cannot be empty.")
                        .GreaterThan(0)
                        .WithMessage("Unit price must be greater than 0.");
                    lines.RuleFor(x => x.TotalPrice)
                        .NotEmpty()
                        .WithMessage("Total price cannot be empty.")
                        .GreaterThan(0)
                        .WithMessage("Total price must be greater than 0.")
                        .Equal(x => x.Quantity * x.UnitPrice)
                        .WithMessage("Total price must be equal to Quantity * Unit Price.");
                });
        }
    }

}
