using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

namespace Seed.Domain.Tests
{
    public class DbContextHelper
    {
        public static DbContextOptions<TDbContext> GetOptions<TDbContext>(string databaseName)
            where TDbContext : DbContext
        {
            return new DbContextOptionsBuilder<TDbContext>()
                .UseInMemoryDatabase(databaseName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // InMemory does not support transactions, ignore this warning
                .Options;
        }

        public static DbContextOptions<TDbContext> GetOptions<TDbContext>()
            where TDbContext : DbContext
        {
            var randomDatabaseName = Guid.NewGuid().ToString();
            return GetOptions<TDbContext>(randomDatabaseName);
        }
    }
}
