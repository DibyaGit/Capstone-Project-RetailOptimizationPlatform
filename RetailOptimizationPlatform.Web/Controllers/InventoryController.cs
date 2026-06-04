using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;
using RetailOptimizationPlatform.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailOptimizationPlatform.Web.Controllers
{
    public class InventoryController(IInventoryRepository inventoryRepository) : Controller
    {
        // GET: /Inventory (Shows the list of all items, includes Search functionality)
        public async Task<IActionResult> Index(string searchString)
        {
            var inventory = await inventoryRepository.GetAllAsync();

            // If the user typed something in the search bar, filter the list
            if (!string.IsNullOrEmpty(searchString))
            {
                inventory = inventory.Where(i => i.ItemName.Contains(searchString, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(inventory);
        }

        // GET: /Inventory/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                await inventoryRepository.AddAsync(inventory);
                return RedirectToAction(nameof(Index));
            }
            return View(inventory);
        }

        // GET: /Inventory/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var inventory = await inventoryRepository.GetByIdAsync(id);
                return View(inventory);
            }
            catch (KeyNotFoundException)
            {
                throw new InventoryNotFoundException(id);
            }
        }

        // POST: /Inventory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inventory inventory)
        {
            if (id != inventory.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await inventoryRepository.UpdateAsync(inventory);
                return RedirectToAction(nameof(Index));
            }

            return View(inventory);
        }

        // GET: /Inventory/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var inventory = await inventoryRepository.GetByIdAsync(id);
                return View(inventory);
            }
            catch (KeyNotFoundException)
            {
                throw new InventoryNotFoundException(id);
            }
        }

        // POST: /Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await inventoryRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Inventory item deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                TempData["ErrorMessage"] = "Cannot delete this item because it is referenced in existing customer orders.";
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting item: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Inventory/GetItemDetails/5 (AJAX Endpoint)
        [HttpGet]
        public async Task<IActionResult> GetItemDetails(int id)
        {
            try
            {
                var item = await inventoryRepository.GetByIdAsync(id);
                return Json(new
                {
                    itemName = item.ItemName,
                    sku = item.Sku,
                    stockQuantity = item.StockQuantity,
                    price = item.Price
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}