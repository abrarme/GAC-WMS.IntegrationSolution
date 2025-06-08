using GAC_WMS.IntegrationSolution.Data;
using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace GAC_WMS.IntegrationSolution.Repositories.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(AppDbContext dbContext, ILogger<ProductRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task BulkInsertAsync(IEnumerable<Product> products)
        {
            if (products == null || !products.Any())
                return;

            var incomingCodes = products
                .Where(p => !string.IsNullOrWhiteSpace(p.ProductCode))
                .Select(p => p.ProductCode)
                .ToHashSet();

            var existingCodes = await _dbContext.Products
                .Where(p => incomingCodes.Contains(p.ProductCode))
                .Select(p => p.ProductCode)
                .ToListAsync();

            var newProducts = products
                .Where(p => !existingCodes.Contains(p.ProductCode))
                .ToList();

            var duplicates = products
                .Where(p => existingCodes.Contains(p.ProductCode))
                .ToList();

            // Log duplicates
            if (duplicates.Any())
            {
                foreach (var dup in duplicates)
                {
                    _logger.LogWarning("Duplicate skipped: {ProductCode} - {Title}", dup.ProductCode, dup.Title);
                }
            }

            if (newProducts.Any())
            {
                await _dbContext.Products.AddRangeAsync(newProducts);
                await _dbContext.SaveChangesAsync();
            }

        }

        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
