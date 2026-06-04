using RetailOptimizationPlatform.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailOptimizationPlatform.Core.Interfaces
{
    /// <summary>
    /// Service layer for handling inventory business logic.
    /// </summary>
    public interface IInventoryService
    {
        Task<IEnumerable<Inventory>> GetAllInventoryAsync();

        /// <summary>
        /// Business rule: Calculates and retrieves items that fall below a specific stock threshold.
        /// </summary>
        Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold = 10);
    }
}