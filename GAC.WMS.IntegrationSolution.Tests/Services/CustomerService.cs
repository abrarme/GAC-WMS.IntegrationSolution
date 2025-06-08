using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Repositories.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAC.WMS.IntegrationSolution.Tests.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task BulkInsertAsync(IEnumerable<Customer> customers);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository repository, ILogger<CustomerService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task<IEnumerable<Customer>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Customer> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task AddAsync(Customer customer) => _repository.AddAsync(customer);
        public Task BulkInsertAsync(IEnumerable<Customer> customers) => _repository.BulkInsertAsync(customers);
        public Task UpdateAsync(Customer customer) => _repository.UpdateAsync(customer);
        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
    }
}
