using Microsoft.AspNetCore.Mvc;
using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailOptimizationPlatform.Web.Controllers
{
    /// <summary>
    /// Suppliers controller for managing supplier information and relationships.
    /// Provides CRUD operations for supplier records with comprehensive validation.
    /// </summary>
    public class SuppliersController : Controller
    {
        private readonly ISupplierRepository _supplierRepository;

        public SuppliersController(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        /// <summary>
        /// GET: /Suppliers (displays list of all suppliers)
        /// </summary>
        public async Task<IActionResult> Index(string searchString)
        {
            IEnumerable<Supplier> suppliers;

            if (string.IsNullOrEmpty(searchString))
            {
                suppliers = await _supplierRepository.GetAllAsync();
            }
            else
            {
                suppliers = await _supplierRepository.SearchAsync(searchString);
            }

            ViewBag.SearchString = searchString;
            return View(suppliers);
        }

        /// <summary>
        /// GET: /Suppliers/Create (displays form to create new supplier)
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: /Suppliers/Create (saves new supplier)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _supplierRepository.AddAsync(supplier);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating supplier: {ex.Message}");
                }
            }
            return View(supplier);
        }

        /// <summary>
        /// GET: /Suppliers/Edit/5 (displays form to edit supplier)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var supplier = await _supplierRepository.GetByIdAsync(id.Value);
            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        /// <summary>
        /// POST: /Suppliers/Edit/5 (updates supplier)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier supplier)
        {
            if (id != supplier.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _supplierRepository.UpdateAsync(supplier);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating supplier: {ex.Message}");
                }
            }
            return View(supplier);
        }

        /// <summary>
        /// GET: /Suppliers/Delete/5 (displays delete confirmation)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var supplier = await _supplierRepository.GetByIdAsync(id.Value);
            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        /// <summary>
        /// POST: /Suppliers/Delete/5 (confirms and deletes supplier)
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _supplierRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Supplier deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting supplier: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// GET: /Suppliers/Details/5 (displays supplier details)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var supplier = await _supplierRepository.GetByIdAsync(id.Value);
            if (supplier == null)
                return NotFound();

            return View(supplier);
        }
    }
}
