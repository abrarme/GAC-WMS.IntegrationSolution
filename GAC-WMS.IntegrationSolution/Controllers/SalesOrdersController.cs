using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Repositories.Implementation;
using GAC_WMS.IntegrationSolution.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using static GAC_WMS.IntegrationSolution.DTO.PurchaseOrder;
using static GAC_WMS.IntegrationSolution.DTO.SalesOrder;

namespace GAC_WMS.IntegrationSolution.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ISalesOrderRepository _repository;
        private readonly ILogger<SalesOrdersController> _logger;
        private readonly ICustomerRepository _customerRepository;
        public SalesOrdersController(ISalesOrderRepository repository, ILogger<SalesOrdersController> logger, ICustomerRepository customerRepository)
        {
            _repository = repository;
            _logger = logger;
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _repository.GetAllAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Gets a sales order by ID.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <returns>Sales order details.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        /// <summary>
        /// Gets a sales order by customerIdentifier.
        /// </summary>
        /// <param name="customerIdentifier">customerIdentifier.</param>
        /// <returns>Sales order details.</returns>
        [HttpGet("customer-identifier/{customerIdentifier}")]
        public async Task<IActionResult> GetByCustomerIdentifier(string customerIdentifier)
        {
            var orders = await _repository.GetOrdersByCustomerIdentifierAsync(customerIdentifier);
            if (!orders.Any()) return NotFound("No sales orders found for this customer.");
            return Ok(orders);
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] SalesOrderDto dto)
        {
            try
            {
                // Find customer by identifier
                var customer = await _customerRepository.GetByIdentifierAsync(dto.CustomerIdentifier);
                if (customer == null)
                {
                    return BadRequest($"Customer with identifier '{dto.CustomerIdentifier}' not found.");
                }

                // Map DTO to Entity
                var order = new SalesOrder
                {
                    OrderId = dto.OrderId,
                    ProcessingDate = dto.ProcessingDate,
                    CustomerIdentifier = customer.CustomerIdentifier,
                    ShipmentAddress = dto.ShipmentAddress,
                    Items = dto.Items.Select(i => new SalesOrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
                };

                await _repository.AddAsync(order);

                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating Sales order");
                return StatusCode(500, new { status = 500, message = "An unexpected error occurred." });
            }


        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SalesOrder salesOrder)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != salesOrder.Id)
                return BadRequest("ID mismatch.");

            await _repository.UpdateAsync(salesOrder);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
