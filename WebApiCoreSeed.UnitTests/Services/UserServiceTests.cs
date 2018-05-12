using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApiCoreSeed.Data.EF;
using WebApiCoreSeed.Data.Models;
using WebApiCoreSeed.Domain.Services;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<WebApiCoreSeedContext>();
            optionsBuilder.UseInMemoryDatabase("GetByIdAsync_ShouldReturnUser");
            var createdUser =
                new User
                {
                    Id = new Guid("d58893fd-3836-4308-b840-85f4fe548264"),
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "JohnDoe@makinsense.com",
                    UserName = "JohnDoe"
                };

            User retrievedUser = null;
            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                context.Add(createdUser);
                context.SaveChanges();
            }

            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                var userService = new UserService(context);

                // Act
                retrievedUser = await userService.GetByIdAsync(createdUser.Id);
            }

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(createdUser.UserName, retrievedUser.UserName);
            Assert.Equal(createdUser.Email, retrievedUser.Email);
            Assert.Equal(createdUser.FirstName, retrievedUser.FirstName);
            Assert.Equal(createdUser.LastName, retrievedUser.LastName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<WebApiCoreSeedContext>();
            optionsBuilder.UseInMemoryDatabase("GetByIdAsync_ShouldReturnNull");
            User user;
            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                var userService = new UserService(context);

                // Act
                user = await userService.GetByIdAsync(Guid.Parse("00000000-0000-0000-0000-000000038479"));
            }

            // Assert
            Assert.Null(user);
        }
    }
}
