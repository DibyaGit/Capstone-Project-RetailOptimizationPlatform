using System.Collections.Generic;
using System.Threading.Tasks;
using RetailOptimizationPlatform.Core.Entities;

namespace RetailOptimizationPlatform.Core.Interfaces
{
    /// <summary>
    /// Repository interface for Supplier entity operations.
    /// Provides data access abstraction for supplier management.
    /// </summary>
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<IEnumerable<Supplier>> GetActiveSuppliesAsync();
        Task<Supplier> GetByIdAsync(int id);
        Task AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(int id);
        Task<IEnumerable<Supplier>> SearchAsync(string searchTerm);
    }
}
