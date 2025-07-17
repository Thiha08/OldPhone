using OldPhone.Core.Shared.Entities;

namespace OldPhone.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository interface for basic CRUD operations
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <typeparam name="TKey">The entity's ID type.</typeparam>
    public interface IRepository<T, TKey> where T : BaseEntity<TKey>
    {
        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">The entity ID</param>
        /// <returns>The entity if found, null otherwise</returns>
        Task<T?> GetByIdAsync(TKey id);

        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>Collection of all entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Adds a new entity
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>The added entity</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <returns>The updated entity</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity by its ID
        /// </summary>
        /// <param name="id">The entity ID</param>
        /// <returns>True if deleted, false otherwise</returns>
        Task<bool> DeleteAsync(TKey id);

        /// <summary>
        /// Checks if an entity exists by its ID
        /// </summary>
        /// <param name="id">The entity ID</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsAsync(string id);
    }
} 