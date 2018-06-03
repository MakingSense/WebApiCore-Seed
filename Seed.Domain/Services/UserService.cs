using Microsoft.EntityFrameworkCore;
using Seed.Data.EF;
using Seed.Data.Models;
using Seed.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seed.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly WebApiCoreSeedContext _dbContext;

        public UserService(WebApiCoreSeedContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetByIdAsync(Guid userId)
        {
            return await _dbContext.Users.FindAsync(userId);
        }

        public async Task<List<User>> GetAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            user.Id = Guid.NewGuid();
            user.CreatedOn = DateTime.Now;

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            var userToUpdate = await _dbContext.Users.FindAsync(user.Id);

            if (userToUpdate == null) return null;

            userToUpdate.Email = user.Email;
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.UserName = user.UserName;
            userToUpdate.UpdatedOn = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return userToUpdate;
        }

        public async Task<bool> DeleteAsync(Guid userId)
        {
            var userToDelete = await _dbContext.Users.FindAsync(userId);

            if (userToDelete == null) return false;

            _dbContext.Users.Remove(userToDelete);

            var result = await _dbContext.SaveChangesAsync();

            return result > 0;
        }
    }
}
