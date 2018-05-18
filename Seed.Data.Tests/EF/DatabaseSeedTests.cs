using Microsoft.EntityFrameworkCore;
using Seed.Data.EF;
using System.Threading.Tasks;
using Xunit;

namespace Seed.Data.Tests.EF
{
    public class DatabaseSeedTests
    {
        [Fact]
        public async Task Initialize_ShouldCreateAUser()
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebApiCoreSeedContext>();
            optionsBuilder.UseInMemoryDatabase("GetByIdAsync_ShouldReturnUser");
            using (var dbContext = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                DatabaseSeed.Initialize(dbContext);

                Assert.Equal(1, await dbContext.Users.CountAsync());
            }
        }
    }
}
