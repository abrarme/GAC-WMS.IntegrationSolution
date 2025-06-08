namespace GAC_WMS.IntegrationSolution.Services.Interface
{
    public interface IFileProcessor
    {
        Task ProcessAsync(string filePath, string endPoint);
    }
}
