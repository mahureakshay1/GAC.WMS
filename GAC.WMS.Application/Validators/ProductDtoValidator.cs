using FluentValidation;
using GAC.WMS.Application.Dtos;

namespace GAC.WMS.Application.Validators
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Product code is required.")
                .Length(1, 50)
                .WithMessage("Product code must be between 1 and 50 characters.");
            RuleFor(x => x.Title).NotEmpty()
                .WithMessage("Product title is required.")
                .Length(1, 100)
                .WithMessage("Product title must be between 1 and 100 characters.");
            RuleFor(x => x.Description).Length(0, 500)
                .WithMessage("Product description must be less than 500 characters.");
            RuleFor(x => x.Length).NotEmpty()
                .WithMessage("Product length is required.")
                .GreaterThan(0)
                .WithMessage("Product length must be greater than 0.");
            RuleFor(x => x.Width).NotEmpty()
                .WithMessage("Product width is required.")
                .GreaterThan(0)
                .WithMessage("Product width must be greater than 0.");
            RuleFor(x => x.Height).NotEmpty()
                .WithMessage("Product height is required.")
                .GreaterThan(0)
                .WithMessage("Product height must be greater than 0.");
            RuleFor(x => x.Weight).NotEmpty()
                .WithMessage("Product weight is required.")
                .GreaterThan(0)
                .WithMessage("Product weight must be greater than 0.");
            RuleFor(x => x.UnitOfDimension).NotNull()
                .WithMessage("Product unit of dimension is required.")
                .IsInEnum()
                .WithMessage("Product unit of dimension is invalid.");
            RuleFor(x => x.QuantityAvailable).NotNull()
                .WithMessage("Quantity  is required.")
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");
            RuleFor(x => x.UnitOfQuantity).NotNull()
               .WithMessage("Product unit of quantity is required.")
               .IsInEnum()
               .WithMessage("Product unit of quantity is invalid.");
        }
    }
}
