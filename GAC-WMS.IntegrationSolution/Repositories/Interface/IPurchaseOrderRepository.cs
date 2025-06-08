using GAC_WMS.IntegrationSolution.Models;

namespace GAC_WMS.IntegrationSolution.Repositories.Interface
{
    public interface IPurchaseOrderRepository
    {
        Task<IEnumerable<PurchaseOrder>> GetAllAsync();
        Task<PurchaseOrder> GetByIdAsync(int id);

        Task<IEnumerable<PurchaseOrder>> GetOrdersByCustomerIdentifierAsync(string customerIdentifier);
        Task AddAsync(PurchaseOrder purchaseOrder);
        Task UpdateAsync(PurchaseOrder purchaseOrder);  
        Task DeleteAsync(int id);
    }
}
    