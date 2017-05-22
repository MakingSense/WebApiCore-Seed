namespace WebApiCoreSeed.Domain.Services.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using WebApiCoreSeed.Data.Models;

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
    }
}