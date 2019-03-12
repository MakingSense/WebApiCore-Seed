using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seed.Data.EF;
using Seed.Data.Models;

namespace Seed.Domain.Services
{
    public abstract class BaseService<T> where T : BaseEntity
    {
        private readonly WebApiCoreSeedContext<T> _dbContext;

        public BaseService(WebApiCoreSeedContext<T> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Entities.FindAsync(id);
        }

        public async Task<List<T>> GetAsync()
        {
            return await _dbContext.Entities.ToListAsync();
        }

        public async Task<T> CreateAsync(T entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedOn = DateTime.Now;

            await _dbContext.Entities.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public abstract Task<T> UpdateAsync(T entity);

        public async Task<bool> DeleteAsync(Guid id)
        {
            var userToDelete = await _dbContext.Entities.FindAsync(id);

            if (userToDelete == null) return false;

            _dbContext.Entities.Remove(userToDelete);

            var result = await _dbContext.SaveChangesAsync();

            return result > 0;
        }
    }
}
