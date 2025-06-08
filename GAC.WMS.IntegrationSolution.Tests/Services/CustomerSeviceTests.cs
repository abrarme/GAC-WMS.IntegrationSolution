using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Repositories.Interface;
using GAC_WMS.IntegrationSolution.Services;
using GAC.WMS.IntegrationSolution.Tests.Services; // Update with correct namespace

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repoMock;
    private readonly Mock<ILogger<CustomerService>> _loggerMock;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _repoMock = new Mock<ICustomerRepository>();
        _loggerMock = new Mock<ILogger<CustomerService>>();
        _service = new CustomerService(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCustomers()
    {
        var customers = new List<Customer> { new Customer { Id = 1, Name = "Test", Email = "a@b.com" } };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

        var result = await _service.GetAllAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCustomer()
    {
        var customer = new Customer { Id = 1, Name = "Test", Email = "a@b.com" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        _repoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldCallRepository()
    {
        var customer = new Customer { Id = 1, Name = "New", Email = "new@a.com" };

        await _service.AddAsync(customer);

        _repoMock.Verify(r => r.AddAsync(customer), Times.Once);
    }

    [Fact]
    public async Task BulkInsertAsync_ShouldCallRepository()
    {
        var customers = new List<Customer>
        {
            new Customer { CustomerIdentifier = "C1", Name = "C1", Email = "c1@mail.com" }
        };

        await _service.BulkInsertAsync(customers);

        _repoMock.Verify(r => r.BulkInsertAsync(customers), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallRepository()
    {
        var customer = new Customer { Id = 1, Name = "Updated", Email = "updated@a.com" };

        await _service.UpdateAsync(customer);

        _repoMock.Verify(r => r.UpdateAsync(customer), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository()
    {
        await _service.DeleteAsync(1);

        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}
