using GAC_WMS.IntegrationSolution.Data;
using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GAC_WMS.IntegrationSolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {

        private readonly ICustomerRepository _repository;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerRepository repository, ILogger<CustomersController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        // GET: api/Customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: api/customer (single)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer customer)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _repository.AddAsync(customer);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        // POST: api/customers/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsert([FromBody] List<Customer> customers)
        {
            if (customers == null || customers.Count == 0)
                return BadRequest("Customer list cannot be empty.");

            await _repository.BulkInsertAsync(customers);
            return Ok(new { message = "Bulk insert successful", count = customers.Count });
        }

        // PUT: api/customers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Customer customer)
        {
            if (id != customer.Id) return BadRequest("ID mismatch.");
            await _repository.UpdateAsync(customer);
            return NoContent();
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _repository.DeleteAsync(id);
            return NoContent();
        }

    }

}
