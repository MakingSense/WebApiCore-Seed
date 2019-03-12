using Microsoft.EntityFrameworkCore;
using Seed.Data.EF;
using Seed.Data.Models;
using Seed.Domain.Services.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seed.Domain.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly UserContext _userContext;

        public UserService(UserContext dbContext) : base(dbContext)
        {
            _userContext = dbContext;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _userContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        }

        public override async Task<User> UpdateAsync(User user)
        {
            var userToUpdate = await _userContext.Users.FindAsync(user.Id);

            if (userToUpdate == null) return null;

            userToUpdate.Email = user.Email;
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.UserName = user.UserName;
            userToUpdate.UpdatedOn = DateTime.Now;

            await _userContext.SaveChangesAsync();

            return userToUpdate;
        }
    }
}
