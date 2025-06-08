using GAC_WMS.IntegrationSolution.Models;

namespace GAC_WMS.IntegrationSolution.Repositories.Interface
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task<Product> GetByProductCode(string productCode);
        Task AddAsync(Product product);
        Task BulkInsertAsync(IEnumerable<Product> products);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
