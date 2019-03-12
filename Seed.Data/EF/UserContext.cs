using Microsoft.EntityFrameworkCore;
using Seed.Data.Models;

namespace Seed.Data.EF
{
    public class UserContext : WebApiCoreSeedContext<User>
    {
        public DbSet<User> Users;

        public UserContext(DbContextOptions options, DbSet<User> users) : base(options, users)
        {
            Users = users;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Table names should be singular but DbSet properties should be plural.
            modelBuilder.Entity<User>().ToTable("User").HasIndex(x => x.UserName).IsUnique();
        }
    }
}
