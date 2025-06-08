using GAC_WMS.IntegrationSolution.Models;
using GAC_WMS.IntegrationSolution.Repositories.Implementation;
using GAC_WMS.IntegrationSolution.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using static GAC_WMS.IntegrationSolution.DTO.PurchaseOrder;

namespace GAC_WMS.IntegrationSolution.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderRepository _repository;
        private readonly ILogger<PurchaseOrdersController> _logger;
        private readonly ICustomerRepository _customerRepository;

        public PurchaseOrdersController(IPurchaseOrderRepository repository, ILogger<PurchaseOrdersController> logger,ICustomerRepository customerRepository)
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
        /// Gets a purchase order by ID.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <returns>Purchase order details.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {

            var order = await _repository.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        /// <summary>
        /// Gets a purchase order by CustomerIdentifier.
        /// </summary>
        /// <param name="customerIdentifier">customerIdentifier.</param>
        /// <returns>Purchase order details.</returns>
        [HttpGet("customer-identifier/{customerIdentifier}")]
        public async Task<IActionResult> GetByCustomerIdentifier(string customerIdentifier)
        {
            var orders = await _repository.GetOrdersByCustomerIdentifierAsync(customerIdentifier);
            if (!orders.Any()) return NotFound("No purchase orders found for this customer.");
            return Ok(orders);
        }

        /// <summary>
        /// Creates a new Purchase Order.
        /// </summary>
        /// <param name="dto">Purchase order details.</param>
        /// <returns>Created order.</returns>

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] PurchaseOrderDto dto)
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
                var order = new PurchaseOrder
                {
                    OrderId = dto.OrderId,
                    ProcessingDate = dto.ProcessingDate,
                    CustomerIdentifier = customer.CustomerIdentifier,
                    Items = dto.Items.Select(i => new PurchaseOrderItem
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
                _logger.LogError(ex, "Error while creating purchase order");
                return StatusCode(500, new { status = 500, message = "An unexpected error occurred." });
            }

      
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PurchaseOrder purchaseOrder)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != purchaseOrder.Id)
                return BadRequest("ID mismatch.");

            await _repository.UpdateAsync(purchaseOrder);
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
