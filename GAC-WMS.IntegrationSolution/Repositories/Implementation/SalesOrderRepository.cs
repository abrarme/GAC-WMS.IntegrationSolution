using GAC_WMS.IntegrationSolution.Data;
using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace GAC_WMS.IntegrationSolution.Repositories.Implementation
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<SalesOrderRepository> _logger;

        public SalesOrderRepository(AppDbContext dbContext, ILogger<SalesOrderRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<SalesOrder>> GetAllAsync()
        {
            return await _dbContext.SalesOrders.ToListAsync();
        }

        public async Task<SalesOrder> GetByIdAsync(int id)
        {
           // return await _dbContext.SalesOrders.FindAsync(id);

            return  await _dbContext.SalesOrders
            .Include(s => s.Items)
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<SalesOrder>> GetOrdersByCustomerIdentifierAsync(string customerIdentifier)
        {
            return await _dbContext.SalesOrders
                .Include(so => so.Items)
                .Include(so => so.Customer)
                .Where(so => so.CustomerIdentifier == customerIdentifier)
                .ToListAsync();
        }

        public async Task AddAsync(SalesOrder salesOrder)
        {
            await _dbContext.SalesOrders.AddAsync(salesOrder);
            await _dbContext.SaveChangesAsync();
        }

     
        public async Task UpdateAsync(SalesOrder salesOrder)
        {
            _dbContext.SalesOrders.Update(salesOrder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var salesOrder = await _dbContext.SalesOrders.FindAsync(id);
            if (salesOrder != null)
            {
                _dbContext.SalesOrders.Remove(salesOrder);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
