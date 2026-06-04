using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailOptimizationPlatform.Core.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;

        // Dependency Injection: The Service talks to the Repository
        public InventoryService(IInventoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Inventory>> GetAllInventoryAsync()
        {
            return await _repository.GetAllAsync();
        }

        // The actual business logic is now isolated here!
        public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold = 10)
        {
            var allItems = await _repository.GetAllAsync();
            return allItems.Where(item => item.StockQuantity <= threshold).ToList();
        }
    }
}