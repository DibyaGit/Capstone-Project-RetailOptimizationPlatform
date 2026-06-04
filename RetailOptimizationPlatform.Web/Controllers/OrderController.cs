using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;
using System.Threading.Tasks;
using System.Linq;

namespace RetailOptimizationPlatform.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IInventoryRepository _inventoryRepository;

        // Constructor injecting both repositories
        public OrderController(IOrderRepository orderRepository, IInventoryRepository inventoryRepository)
        {
            _orderRepository = orderRepository;
            _inventoryRepository = inventoryRepository;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAllAsync();
            return View(orders);
        }

        // GET: Order/Create
        public async Task<IActionResult> Create()
        {
            var items = await _inventoryRepository.GetAllAsync();
            ViewBag.InventoryList = new SelectList(items, "Id", "ItemName");
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerName,OrderDate,TotalAmount,InventoryId")] Order order)
        {
            if (ModelState.IsValid)
            {
                // Verify the selected inventory item actually exists
                Inventory? item = null;
                try
                {
                    item = await _inventoryRepository.GetByIdAsync(order.InventoryId);
                }
                catch (KeyNotFoundException)
                {
                    // Item not found, handled by null check below
                }

                if (item == null)
                {
                    ModelState.AddModelError("InventoryId", "The selected inventory item does not exist.");
                    var items = await _inventoryRepository.GetAllAsync();
                    ViewBag.InventoryList = new SelectList(items, "Id", "ItemName", order.InventoryId);
                    return View(order);
                }

                try
                {
                    await _orderRepository.AddAsync(order);
                    TempData["SuccessMessage"] = "Order created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error creating order: {ex.Message}");
                    var items = await _inventoryRepository.GetAllAsync();
                    ViewBag.InventoryList = new SelectList(items, "Id", "ItemName", order.InventoryId);
                    return View(order);
                }
            }

            // If we fail validation, reload the dropdown list before returning the view
            var fallbackItems = await _inventoryRepository.GetAllAsync();
            ViewBag.InventoryList = new SelectList(fallbackItems, "Id", "ItemName", order.InventoryId);
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var items = await _inventoryRepository.GetAllAsync();
            ViewBag.InventoryList = new SelectList(items, "Id", "ItemName", order.InventoryId);
            return View(order);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerName,OrderDate,TotalAmount,InventoryId")] Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // Verify the selected inventory item actually exists
                Inventory? item = null;
                try
                {
                    item = await _inventoryRepository.GetByIdAsync(order.InventoryId);
                }
                catch (KeyNotFoundException)
                {
                    // Item not found, handled by null check below
                }

                if (item == null)
                {
                    ModelState.AddModelError("InventoryId", "The selected inventory item does not exist.");
                    var items = await _inventoryRepository.GetAllAsync();
                    ViewBag.InventoryList = new SelectList(items, "Id", "ItemName", order.InventoryId);
                    return View(order);
                }

                try
                {
                    await _orderRepository.UpdateAsync(order);
                    TempData["SuccessMessage"] = "Order updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error saving order: {ex.Message}");
                    var items = await _inventoryRepository.GetAllAsync();
                    ViewBag.InventoryList = new SelectList(items, "Id", "ItemName", order.InventoryId);
                    return View(order);
                }
            }

            // If we fail validation, reload the dropdown list before returning the view
            var fallbackItems = await _inventoryRepository.GetAllAsync();
            ViewBag.InventoryList = new SelectList(fallbackItems, "Id", "ItemName", order.InventoryId);
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _orderRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Order deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting order: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}