using Seed.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seed.Domain.Services.Interfaces
{
    /// <summary>
    /// Service responsible of handling users
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">Id of the user to be retrieved</param>
        /// <returns>A <see cref="User"/> object if the user is found, otherwise null</returns>
        Task<User> GetByIdAsync(Guid userId);

        /// <summary>
        /// Creates a user
        /// </summary>
        /// <param name="user">User to create</param>
        /// <returns>Created user</returns>
        Task<User> CreateAsync(User user);

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="userId">Id of the user to delete</param>
        /// <returns>Whether the user was deleted or not</returns>
        Task<bool> DeleteAsync(Guid userId);

        /// <summary>
        /// Gets all the existing users
        /// </summary>
        /// <returns>List with all the existing users</returns>
        Task<List<User>> GetAsync();

        /// <summary>
        /// Updates a user
        /// </summary>
        /// <param name="user">User to update</param>
        /// <returns>Updated user</returns>
        Task<User> UpdateAsync(User user);
    }
}