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
    public class ProductsController : ControllerBase
    {

        private readonly IProductRepository _repository;
        private readonly ILogger<ProductsController> _logger;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository repository, ILogger<ProductsController> logger,IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _repository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(result);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)    
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpGet("product-code/{productcode}")]
        public async Task<ActionResult<Customer>> GetByIdentifier(string productcode)
        {
            var product = await _repository.GetByProductCode(productcode);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: api/products (single)
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _repository.AddAsync(product);
            return CreatedAtAction(nameof(GetAll), new { id = product.Id }, dto);
        }

        // POST: api/products/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsert([FromBody] List<Product> products)
        {
            if (products == null || products.Count == 0)
                return BadRequest("Product list cannot be empty.");

            await _repository.BulkInsertAsync(products);
            return Ok(new { message = "Bulk insert successful", count = products.Count });
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return NotFound();

            _mapper.Map(dto, product);
            await _repository.UpdateAsync(product);
            return NoContent();
        }

        // DELETE: api/products/{id}
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
