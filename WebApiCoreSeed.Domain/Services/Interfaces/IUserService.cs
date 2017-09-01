using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiCoreSeed.Data.Models;

namespace WebApiCoreSeed.Domain.Services.Interfaces
{
    /// <summary>
    /// Service responsible of handling users
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets a user by Id
        /// </summary>
        /// <param name="userId">Id of the user to be retrieved</param>
        /// <returns>A <see cref="User"/> object if the user is found, otherwise null</returns>
        Task<User> GetByIdAsync(Guid userId);

        Task<int> CreateAsync(User user);
        Task<int> DeleteByIdAsync(Guid userId);
        Task<List<User>> GetAsync();
        Task<int> UpdateAsync(User user);
    }
}