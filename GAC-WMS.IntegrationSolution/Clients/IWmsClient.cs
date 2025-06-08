using GAC_WMS.IntegrationSolution.Models;

namespace GAC_WMS.IntegrationSolution.Clients
{
    public interface IWmsClient
    {
        Task PushDataAsync(IEnumerable<dynamic> list,string endPoint);
    }

}
