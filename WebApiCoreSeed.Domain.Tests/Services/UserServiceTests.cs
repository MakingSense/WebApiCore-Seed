using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiCoreSeed.Data.EF;
using WebApiCoreSeed.Data.Models;
using WebApiCoreSeed.Domain.Services;
using Xunit;

namespace WebApiCoreSeed.Domain.Tests.Services
{
    public class UserServiceTests
    {
        #region GetById Tests

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var createdUser = new User
            {
                Id = new Guid("d58893fd-3836-4308-b840-85f4fe548264"),
                FirstName = "John",
                LastName = "Doe",
                Email = "JohnDoe@makinsense.com",
                UserName = "JohnDoe"
            };

            var optionsBuilder = GetBuilder(createdUser);

            User retrievedUser = null;
            

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
            var optionsBuilder = GetBuilder();
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

        #endregion

        #region Private Methods

        private DbContextOptionsBuilder<WebApiCoreSeedContext> GetBuilder()
        {
            return GetBuilder(Guid.NewGuid().ToString(), null);
        }

        private DbContextOptionsBuilder<WebApiCoreSeedContext> GetBuilder(User user)
        {
            return GetBuilder(Guid.NewGuid().ToString(), new List<User>() { user });
        }

        private DbContextOptionsBuilder<WebApiCoreSeedContext> GetBuilder(IEnumerable<User> users)
        {
            return GetBuilder(Guid.NewGuid().ToString(), users);
        }


        private DbContextOptionsBuilder<WebApiCoreSeedContext> GetBuilder(string memoryContext, IEnumerable<User> users)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebApiCoreSeedContext>();
            optionsBuilder.UseInMemoryDatabase(memoryContext);

            if (users != null)
            {
                using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
                {
                    context.AddRange(users);
                    context.SaveChanges();
                }
            }

            return optionsBuilder;
        }

        #endregion
    }
}
