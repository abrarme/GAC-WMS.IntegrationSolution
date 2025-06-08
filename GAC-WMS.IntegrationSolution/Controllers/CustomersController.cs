using AutoMapper;
using GAC_WMS.IntegrationSolution.Data;
using GAC_WMS.IntegrationSolution.DTO;
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
        private readonly IMapper _mapper;

        public CustomersController(ICustomerRepository repository, ILogger<CustomersController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _repository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            return Ok(result);
        }


        // GET: api/Customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetById(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }


        [HttpGet("customer-identifier/{customerIdentifier}")]
        public async Task<ActionResult<Customer>> GetByIdentifier(string customerIdentifier)
        {
            var product = await _repository.GetByIdentifierAsync(customerIdentifier);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: api/customer (single)
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CustomerDto dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            await _repository.AddAsync(customer);
            return CreatedAtAction(nameof(GetAll), new { id = customer.Id }, dto);
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
        public async Task<IActionResult> Update(int id, [FromBody] CustomerDto dto)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null) return NotFound();

            _mapper.Map(dto, customer);
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
