namespace GAC.WMS.Application.Interfaces
{
    public interface IValidatorService<T>
    {
        Task ValidateAsync(T instance, CancellationToken cancellationToken);
    }
}
