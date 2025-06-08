using GAC_WMS.IntegrationSolution.Data;
using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace GAC_WMS.IntegrationSolution.Repositories.Implementation
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CustomerRepository> _logger;

        public CustomerRepository(AppDbContext dbContext, ILogger<CustomerRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _dbContext.Customers.FindAsync(id);
        }

        public async Task AddAsync(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task BulkInsertAsync(IEnumerable<Customer> customers)
        {
            if (customers == null || !customers.Any())
                return;

            var incomingCodes = customers
                .Where(p => !string.IsNullOrWhiteSpace(p.CustomerIdentifier))
                .Select(p => p.CustomerIdentifier)
                .ToHashSet();

            var existingCodes = await _dbContext.Customers
                .Where(p => incomingCodes.Contains(p.CustomerIdentifier))
                .Select(p => p.CustomerIdentifier)
                .ToListAsync();

            var newCustomers = customers
                .Where(p => !existingCodes.Contains(p.CustomerIdentifier))
                .ToList();

            var duplicates = customers
                .Where(p => existingCodes.Contains(p.CustomerIdentifier))
                .ToList();

            // Log duplicates
            if (duplicates.Any())
            {
                foreach (var dup in duplicates)
                {
                    _logger.LogWarning("Duplicate skipped: {CustomerCode} - {Title}", dup.CustomerIdentifier, dup.Name);
                }
            }

            if (newCustomers.Any())
            {
                await _dbContext.Customers.AddRangeAsync(newCustomers);
                await _dbContext.SaveChangesAsync();
            }

        }

        public async Task UpdateAsync(Customer customer)
        {
            _dbContext.Customers.Update(customer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);
            if (customer != null)
            {
                _dbContext.Customers.Remove(customer);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
