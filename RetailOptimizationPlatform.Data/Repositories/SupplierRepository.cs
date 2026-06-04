using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetailOptimizationPlatform.Core.Entities;
using RetailOptimizationPlatform.Core.Interfaces;

namespace RetailOptimizationPlatform.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Supplier entity.
    /// Handles CRUD operations and specialized queries for supplier management.
    /// </summary>
    public class SupplierRepository(ApplicationDbContext context) : ISupplierRepository
    {
        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await context.Suppliers
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Supplier>> GetActiveSuppliesAsync()
        {
            return await context.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Supplier> GetByIdAsync(int id)
        {
            return await context.Suppliers.FindAsync(id)
                ?? throw new KeyNotFoundException($"Supplier with ID {id} was not found.");
        }

        public async Task AddAsync(Supplier supplier)
        {
            supplier.CreatedDate = DateTime.UtcNow;
            supplier.LastUpdated = DateTime.UtcNow;
            await context.Suppliers.AddAsync(supplier);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            supplier.LastUpdated = DateTime.UtcNow;
            context.Suppliers.Update(supplier);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var supplier = await context.Suppliers.FindAsync(id)
                ?? throw new KeyNotFoundException($"Cannot delete. Supplier with ID {id} was not found.");

            context.Suppliers.Remove(supplier);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Supplier>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var term = searchTerm.ToLower();
            return await context.Suppliers
                .Where(s => s.Name.ToLower().Contains(term) ||
                           s.ContactPerson.ToLower().Contains(term) ||
                           s.Email.ToLower().Contains(term) ||
                           s.City.ToLower().Contains(term))
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}
