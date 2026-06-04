using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailOptimizationPlatform.Api.Controllers
{
    /// <summary>
    /// Manages customer orders via REST API.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires a valid JWT token for all endpoints
    public class OrderApiController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderApiController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Retrieves all orders from the database.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderRepository.GetAllAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Retrieves a specific order by its ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { Message = $"Order with ID {id} not found." });
            }
            return Ok(order);
        }

        /// <summary>
        /// Creates a new customer order.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _orderRepository.AddAsync(order);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest(new { Message = "ID mismatch in update request." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _orderRepository.UpdateAsync(order);
            return NoContent();
        }

        /// <summary>
        /// Deletes an order from the system. Requires Admin role.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { Message = $"Order with ID {id} not found." });
            }

            await _orderRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}