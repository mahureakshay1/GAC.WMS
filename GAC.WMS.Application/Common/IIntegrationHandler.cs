namespace GAC.WMS.Application.Common
{
    public interface IIntegrationHandler
    {
        bool CanHandleAsync(string filePath);
        void HandleError(string filePath, string errorMessage);
        void HandleSuccess(string filePath);
        Task ProcessAsync(string filePath, CancellationToken cancellationToken = default);
    }
}
