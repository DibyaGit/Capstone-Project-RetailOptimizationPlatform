using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;

namespace RetailOptimizationPlatform.Data.Repositories
{
    public class InventoryRepository(ApplicationDbContext context) : IInventoryRepository
    {
        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            return await context.Inventories.ToListAsync();
        }

        public async Task<Inventory> GetByIdAsync(int id)
        {
            
            return await context.Inventories.FindAsync(id)
                ?? throw new KeyNotFoundException($"Inventory item with ID {id} was not found.");
        }

        public async Task AddAsync(Inventory inventory)
        {
            await context.Inventories.AddAsync(inventory);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Inventory inventory)
        {
            context.Inventories.Update(inventory);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            
            var inventory = await context.Inventories.FindAsync(id)
                ?? throw new KeyNotFoundException($"Cannot delete. Inventory item with ID {id} was not found.");

            context.Inventories.Remove(inventory);
            await context.SaveChangesAsync();
        }
    }
}