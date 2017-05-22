namespace WebApiCoreSeed.Domain.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using WebApiCoreSeed.Data.EF;
    using WebApiCoreSeed.Domain.Services.Interfaces;
    using WebApiCoreSeed.Data.Models;

    /// <inheritdoc/>
    public class UserService : IUserService
    {
        private readonly WebApiCoreSeedContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="dbContext"><see cref="WebApiCoreSeedContext"/> instance required to access database </param>
        public UserService(WebApiCoreSeedContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<User> GetByIdAsync(Guid userId)
        {
            return await _dbContext.Users.FindAsync(userId);
        }
    }
}
