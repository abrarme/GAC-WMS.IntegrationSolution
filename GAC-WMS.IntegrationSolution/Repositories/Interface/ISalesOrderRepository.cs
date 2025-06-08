using GAC_WMS.IntegrationSolution.Models;

namespace GAC_WMS.IntegrationSolution.Repositories.Interface
{
    public interface ISalesOrderRepository
    {
        Task<IEnumerable<SalesOrder>> GetAllAsync();
        Task<SalesOrder> GetByIdAsync(int id);
        Task<IEnumerable<SalesOrder>> GetOrdersByCustomerIdentifierAsync(string customerIdentifier);
        Task AddAsync(SalesOrder salesOrder);
        Task UpdateAsync(SalesOrder salesOrder);
        Task DeleteAsync(int id);
    }
}
