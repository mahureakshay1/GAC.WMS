using FluentValidation;
using GAC.WMS.Application.Interfaces;

namespace GAC.WMS.Infrastructure.Services
{
    public class ValidatorService<T> : IValidatorService<T>
    {
        private readonly IValidator<T> _validator;

        public ValidatorService(IValidator<T> validator)
        {
            _validator = validator;
        }
        public async Task ValidateAsync(T instance, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(instance, cancellationToken);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }
    }
}
