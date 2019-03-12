using Microsoft.EntityFrameworkCore;
using Seed.Data.Models;

namespace Seed.Data.EF
{
    public class WebApiCoreSeedContext<T> : DbContext where T : BaseEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiCoreSeedContext"/> class.
        ///
        /// DbContextOptions parameter is required by AspNet core initialization
        /// </summary>
        /// <param name="options">Options used to create this <see cref="WebApiCoreSeedContext"/> instance </param>
        public WebApiCoreSeedContext(DbContextOptions options, DbSet<T> entities) : base(options)
        {
            Entities = entities;
        }

        /// <summary> All entities registered on WebApiCoreSeed database</summary>
        public virtual DbSet<T> Entities { get; }
    }
}
