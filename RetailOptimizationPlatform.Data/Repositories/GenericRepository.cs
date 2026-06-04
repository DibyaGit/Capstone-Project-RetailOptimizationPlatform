using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetailOptimizationPlatform.Core.Interfaces;

namespace RetailOptimizationPlatform.Data.Repositories
{
    /// <summary>
    /// Generic repository implementing the Repository pattern for SOLID principles compliance.
    /// Provides base CRUD operations with filtering, pagination, and sorting capabilities.
    /// </summary>
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext Context;
        protected readonly DbSet<T> DbSet;

        protected GenericRepository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        /// <summary>
        /// Retrieves all entities with optional filtering and pagination.
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync(int? pageNumber = null, int? pageSize = null)
        {
            IQueryable<T> query = DbSet;

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                int skip = (pageNumber.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Retrieves a single entity by ID.
        /// </summary>
        public async Task<T> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id)
                ?? throw new KeyNotFoundException($"Entity with ID {id} was not found.");
        }

        /// <summary>
        /// Retrieves entities matching a predicate with optional pagination.
        /// </summary>
        public async Task<IEnumerable<T>> GetByConditionAsync(
            Expression<Func<T, bool>> predicate,
            int? pageNumber = null,
            int? pageSize = null)
        {
            IQueryable<T> query = DbSet.Where(predicate);

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                int skip = (pageNumber.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Adds a single entity.
        /// </summary>
        public async Task<T> AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Adds multiple entities in a single operation.
        /// </summary>
        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await DbSet.AddRangeAsync(entities);
            await Context.SaveChangesAsync();
            return entities;
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        public async Task<T> UpdateAsync(T entity)
        {
            DbSet.Update(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Deletes an entity by ID.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var entity = await DbSet.FindAsync(id)
                ?? throw new KeyNotFoundException($"Cannot delete. Entity with ID {id} was not found.");

            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a specific entity.
        /// </summary>
        public async Task DeleteAsync(T entity)
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the count of entities matching a condition.
        /// </summary>
        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await DbSet.CountAsync();

            return await DbSet.CountAsync(predicate);
        }

        /// <summary>
        /// Checks if any entity matches the condition.
        /// </summary>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }
    }
}
