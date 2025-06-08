using GAC_WMS.IntegrationSolution.Models;

namespace GAC_WMS.IntegrationSolution.Repositories.Interface
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task AddAsync(Customer Customer);
        Task BulkInsertAsync(IEnumerable<Customer> Customers);
        Task UpdateAsync(Customer Customer);
        Task DeleteAsync(int id);
    }
}
