using Seed.Data.Models;
using System.Threading.Tasks;

namespace Seed.Domain.Services.Interfaces
{
    /// <summary>
    /// Service responsible of handling users
    /// </summary>
    public interface IUserService : IBaseService<User>
    {
        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="email">Email of the user to be retrieved</param>
        /// <returns>A <see cref="User"/> object if the user is found, otherwise null</returns>
    }
}