using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCoreSeed.Data.EF;
using WebApiCoreSeed.Data.Enums;
using WebApiCoreSeed.Data.Models;
using WebApiCoreSeed.Domain.Services.Interfaces;

namespace WebApiCoreSeed.Domain.Services
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
            var Users = _dbContext.Users.ToList();
            return Users;            
        }

        public async Task<string> CreateAsync(User user)
        {
            string result = string.Empty;
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            result = CodeResponse.Sucessful.ToString();
            return result;

        }

        public async Task<string> UpdateAsync(User user)
        {
            string result = string.Empty;            
            var userUpdate = (from u in _dbContext.Users
                              where u.Id == user.Id
                              select u).FirstOrDefault();

            if(userUpdate == null)
            {
                result = CodeResponse.Failure.ToString();
                return result;
            }
            userUpdate.FirstName = user.FirstName;
            userUpdate.LastName = user.LastName;
            userUpdate.UserName = user.UserName;
            userUpdate.UpdatedBy = user.UpdatedBy;
            userUpdate.UpdatedOn = DateTime.Now;
              
            int recordsUpdate = _dbContext.SaveChanges();
            if(recordsUpdate == 1)
            {
               result = CodeResponse.Sucessful.ToString();
            }
            else
            {
               result = CodeResponse.Failure.ToString();
            }
            return result;

        }


        public async Task<string> DeleteByIdAsync(Guid userId)
        {
            var user = _dbContext.Users.SingleOrDefault(item => item.Id == userId);
            string result = string.Empty;

            if (user != null)
            {
                // The items exists. So we remove it and calling 
                // the db.SaveChanges this will be removed from the database.
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
                result = CodeResponse.Sucessful.ToString();
            }
            else
            {
                result = CodeResponse.Failure.ToString();
            }
            return result;
           
        }

    }
}
