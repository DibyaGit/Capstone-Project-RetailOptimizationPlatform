using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailOptimizationPlatform.Api.Controllers
{
    /// <summary>
    /// Manages inventory operations via REST API.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryApiController : ControllerBase
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryApiController(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        /// <summary>
        /// Retrieves the complete list of inventory items.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventory()
        {
            var inventory = await _inventoryRepository.GetAllAsync();
            return Ok(inventory);
        }

        /// <summary>
        /// Retrieves a specific inventory item by its unique ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventory(int id)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            if (item == null)
            {
                // API STANDARDIZATION GAP CLOSED: Consistent Error JSON
                return NotFound(new { Error = "Not Found", Message = $"Inventory item with ID {id} was not found." });
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a newly stocked inventory item.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Inventory>> PostInventory(Inventory inventory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Validation Failed", Details = ModelState });
            }

            await _inventoryRepository.AddAsync(inventory);
            return CreatedAtAction(nameof(GetInventory), new { id = inventory.Id }, inventory);
        }

        /// <summary>
        /// Updates the details of an existing inventory item.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventory(int id, Inventory inventory)
        {
            if (id != inventory.Id)
            {
                return BadRequest(new { Error = "Bad Request", Message = "The ID in the URL does not match the ID in the payload." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Validation Failed", Details = ModelState });
            }

            await _inventoryRepository.UpdateAsync(inventory);
            return NoContent();
        }

        /// <summary>
        /// Removes an inventory item from the database. Requires Administrator privileges.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound(new { Error = "Not Found", Message = $"Inventory item with ID {id} was not found." });
            }

            await _inventoryRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}