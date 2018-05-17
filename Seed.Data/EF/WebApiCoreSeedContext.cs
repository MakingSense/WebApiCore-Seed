using Microsoft.EntityFrameworkCore;
using Seed.Data.Models;

namespace Seed.Data.EF
{
    public class WebApiCoreSeedContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiCoreSeedContext"/> class.
        ///
        /// DbContextOptions parameter is required by AspNet core initialization
        /// </summary>
        /// <param name="options">Options used to create this <see cref="WebApiCoreSeedContext"/> instance </param>
        public WebApiCoreSeedContext(DbContextOptions options) : base(options) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiCoreSeedContext"/> class without parameters.
        /// </summary>
        public WebApiCoreSeedContext() : base() { }

        /// <summary> All users registered on WebApiCoreSeed database</summary>
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Table names should be singular but DbSet properties should be plural.
            modelBuilder.Entity<User>().ToTable("User").HasIndex(x => x.UserName).IsUnique();
        }
    }
}
