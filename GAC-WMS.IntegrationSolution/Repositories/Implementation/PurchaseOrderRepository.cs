using GAC_WMS.IntegrationSolution.Data;
using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace GAC_WMS.IntegrationSolution.Repositories.Implementation
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PurchaseOrderRepository> _logger;

        public PurchaseOrderRepository(AppDbContext dbContext, ILogger<PurchaseOrderRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllAsync()
        {
            return await _dbContext.PurchaseOrders.ToListAsync();
        }

        public async Task<PurchaseOrder> GetByIdAsync(int id)
        {
            //return await _dbContext.PurchaseOrders.FindAsync(id);
            return await _dbContext.PurchaseOrders
            .Include(p => p.Items)
            .Include(p => p.Customer)
            .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetOrdersByCustomerIdentifierAsync(string customerIdentifier)
        {
            return await _dbContext.PurchaseOrders
                .Include(po => po.Items)
                .Include(po => po.Customer)
                .Where(po => po.CustomerIdentifier == customerIdentifier)
                .ToListAsync();
        }

        public async Task AddAsync(PurchaseOrder purchaseOrder)
        {
            await _dbContext.PurchaseOrders.AddAsync(purchaseOrder);
            await _dbContext.SaveChangesAsync();
        }

     
        public async Task UpdateAsync(PurchaseOrder purchaseOrder)
        {
            _dbContext.PurchaseOrders.Update(purchaseOrder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var purchaseOrder = await _dbContext.PurchaseOrders.FindAsync(id);
            if (purchaseOrder != null)
            {
                _dbContext.PurchaseOrders.Remove(purchaseOrder);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
