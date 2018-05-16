using Microsoft.EntityFrameworkCore;
using Seed.Data.EF;
using Seed.Data.Models;
using Seed.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seed.Domain.Services
{
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

        public async Task<List<User>> GetAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<int> CreateAsync(User user)
        {
            user.CreatedOn = DateTime.Now;
            _dbContext.Users.Add(user);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(User user)
        {
            var userToUpdate = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            if(userToUpdate == null)
            {
                return 0;
            }

            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.UserName = user.UserName;
            userToUpdate.UpdatedBy = user.UpdatedBy;
            userToUpdate.UpdatedOn = DateTime.Now;
              
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteByIdAsync(Guid userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(item => item.Id == userId);
            if (user == null)
            {
                return 0;
            }

            _dbContext.Users.Remove(user);
            return await _dbContext.SaveChangesAsync();           
        }
    }
}
