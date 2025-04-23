using FluentValidation;
using GAC.WMS.Application.Dtos;

namespace GAC.WMS.Application.Validators
{
    public class CustomerDtoValidator :  AbstractValidator<CustomerDto>
    {
        public CustomerDtoValidator()
        {

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(100).WithMessage("Company name must not exceed 100 characters.");
            RuleFor(x => x.ContactPersonName)
                .NotEmpty().WithMessage("Contact person name is required.")
                .MaximumLength(100).WithMessage("Contact person name must not exceed 100 characters.");
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");
            RuleFor(x => x.Contact)
                .NotEmpty().WithMessage("Contact is required.")
                .MaximumLength(50).WithMessage("Contact must not exceed 50 characters.");
        }
    }
}
