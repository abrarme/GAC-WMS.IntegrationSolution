using Microsoft.AspNetCore.Mvc;


namespace GAC_WMS.IntegrationSolution.Helper
{
    public interface IFileErrorHandler
    {
        void MoveToError(string filePath, Exception ex);
    }
    public class FileErrorHandler : IFileErrorHandler
    {
        public void MoveToError(string filePath, Exception ex)
        {
            FileHelper.MoveToError(filePath, ex);
        }
    }

}
