using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApiCoreSeed.Data.EF;
using Xunit;

namespace WebApiCoreSeed.Data.Tests.EF
{
    public class DatabaseSeedTest
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
