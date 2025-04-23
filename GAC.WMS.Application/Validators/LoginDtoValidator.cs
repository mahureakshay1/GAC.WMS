using FluentValidation;
using GAC.WMS.Application.DTOs;

namespace GAC.WMS.Application.Validators
{
    public class LoginDtoValidator: AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("User name is required.")
                .MaximumLength(50).WithMessage("User name must not exceed 50 characters.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(5).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");
        }
    }
}
