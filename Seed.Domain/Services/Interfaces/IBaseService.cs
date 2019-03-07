using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Seed.Data.Models;

namespace Seed.Domain.Services.Interfaces
{
    public interface IBaseService<T> where T : BaseEntity
    {
        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="guid">Id of the entity to be retrieved</param>
        /// <returns>A <see cref="T"/> object if the entity is found, otherwise null</returns>
        Task<T> GetByIdAsync(Guid guid);

        /// <summary>
        /// Creates a entity
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>Created user</returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Deletes a entity
        /// </summary>
        /// <param name="guid">Id of the entity to delete</param>
        /// <returns>Whether the entity was deleted or not</returns>
        Task<bool> DeleteAsync(Guid guid);

        /// <summary>
        /// Gets all the existing entities
        /// </summary>
        /// <returns>List with all the existing entities</returns>
        Task<List<T>> GetAsync();

        /// <summary>
        /// Updates a entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>Updated entity</returns>
        Task<T> UpdateAsync(T entity);
    }
}
