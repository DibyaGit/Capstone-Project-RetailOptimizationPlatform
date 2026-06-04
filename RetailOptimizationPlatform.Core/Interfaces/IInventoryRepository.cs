using RetailOptimizationPlatform.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailOptimizationPlatform.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for inventory data access operations.
    /// Acts as the abstraction layer between the business logic and the database.
    /// </summary>
    public interface IInventoryRepository
    {
        /// <summary>
        /// Retrieves all inventory items from the database.
        /// </summary>
        /// <returns>An enumerable collection of Inventory entities.</returns>
        Task<IEnumerable<Inventory>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific inventory item by its unique system identifier.
        /// </summary>
        /// <param name="id">The integer ID of the target inventory item.</param>
        /// <returns>The populated Inventory entity if found; otherwise, null.</returns>
        Task<Inventory> GetByIdAsync(int id);

        /// <summary>
        /// Persists a new inventory item to the database.
        /// </summary>
        /// <param name="inventory">The populated Inventory model to save.</param>
        Task AddAsync(Inventory inventory);

        /// <summary>
        /// Updates an existing inventory item's properties in the database.
        /// </summary>
        /// <param name="inventory">The updated Inventory model.</param>
        Task UpdateAsync(Inventory inventory);

        /// <summary>
        /// Removes an inventory item from the system permanently.
        /// </summary>
        /// <param name="id">The integer ID of the target inventory item.</param>
        Task DeleteAsync(int id);
    }
}