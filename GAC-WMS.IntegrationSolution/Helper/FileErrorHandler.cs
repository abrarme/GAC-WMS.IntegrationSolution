using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
