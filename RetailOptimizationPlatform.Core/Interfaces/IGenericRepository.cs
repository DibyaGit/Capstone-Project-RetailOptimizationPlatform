using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RetailOptimizationPlatform.Core.Interfaces
{
    /// <summary>
    /// Generic repository interface implementing Repository pattern.
    /// Provides abstraction for data access operations with support for filtering, pagination, and CRUD.
    /// </summary>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities with optional pagination.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync(int? pageNumber = null, int? pageSize = null);

        /// <summary>
        /// Retrieves a single entity by ID.
        /// </summary>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves entities matching a predicate with optional pagination.
        /// </summary>
        Task<IEnumerable<T>> GetByConditionAsync(
            Expression<Func<T, bool>> predicate,
            int? pageNumber = null,
            int? pageSize = null);

        /// <summary>
        /// Adds a single entity.
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Adds multiple entities.
        /// </summary>
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity by ID.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Deletes a specific entity.
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Gets the count of entities matching a condition.
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// Checks if any entity matches the condition.
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
