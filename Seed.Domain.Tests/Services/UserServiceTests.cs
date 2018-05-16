using Microsoft.EntityFrameworkCore;
using Seed.Data.EF;
using Seed.Data.Models;
using Seed.Domain.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Seed.Domain.Tests
{
    public class UserServiceTests
    {
        #region GetById Tests

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var optionsBuilder = DbContextBuilder();
            var createdUser = GetADefaultUser();

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
            var optionsBuilder = DbContextBuilder("GetByIdAsync_ShouldReturnNull");
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

        #region Detele Tests

        [Fact]
        public async void Delete_ShouldDeleteUser()
        {
            // Arrange
            var optionsBuilder = DbContextBuilder("Delete_ShouldDeleteUser");
            var createdUser = GetADefaultUser();

            int affectedRows;
            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                context.Add(createdUser);
                context.SaveChanges();
            }

            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                var userService = new UserService(context);

                // Act
                affectedRows = await userService.DeleteByIdAsync(createdUser.Id);
            }

            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                // Act
                var user = await context.Users.FindAsync(createdUser.Id);
                // Assert
                Assert.Null(user);
            }
            // Assert
            Assert.True(affectedRows > 0);
        }

        [Fact]
        public async void Delete_UserNotFound()
        {
            // Arrange
            var optionsBuilder = DbContextBuilder("Delete_UserNotFound");
            int affectedRows;
            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                var userService = new UserService(context);

                // Act
                affectedRows = await userService.DeleteByIdAsync(Guid.NewGuid());
            }

            // Assert
            Assert.Equal(0, affectedRows);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async void Update_ShouldUpdateIfUserExists()
        {
            // Arrange
            var optionsBuilder = DbContextBuilder("Update_ShouldUpdateIfUserExists");
            var createdUser = GetADefaultUser();
            int affectedRows;
            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                context.Add(createdUser);
                context.SaveChanges();
            }

            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                var userService = new UserService(context);

                // Act
                affectedRows = await userService.UpdateAsync(createdUser);
            }

            // Assert
            Assert.True(affectedRows > 0);
        }

        [Fact]
        public async void Update_ShouldReturnZeroIfUserNotExists()
        {
            // Arrange
            var optionsBuilder = DbContextBuilder("Update_ShouldReturnZeroIfUserNotExists");
            var createdUser = GetADefaultUser();
            int affectedRows;
            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                var userService = new UserService(context);

                // Act
                affectedRows = await userService.UpdateAsync(createdUser);
            }

            // Assert
            Assert.Equal(0, affectedRows);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async void Create_ShoulReturnOneIfCreated()
        {
            // Arrange
            var optionsBuilder = DbContextBuilder("Create_ShoulReturnOneIfCreated");
            var createdUser = GetADefaultUser();

            int affectedRows;
            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                context.Add(createdUser);
                context.SaveChanges();
            }

            using (var context = new WebApiCoreSeedContext(optionsBuilder.Options))
            {
                var userService = new UserService(context);

                // Act
                affectedRows = await userService.UpdateAsync(createdUser);
                // Assert
                Assert.Equal(1, affectedRows);
            }
        }

        #endregion

        #region Private Methods

        private static User GetADefaultUser(Guid? id = null)
        {
            var sanitizedId = id ?? Guid.NewGuid();

            return new User
            {
                Id = new Guid("d58893fd-3836-4308-b840-85f4fe548264"),
                FirstName = "John",
                LastName = "Doe",
                Email = "JohnDoe@makinsense.com",
                UserName = "JohnDoe"
            };
        }

        private static DbContextOptionsBuilder DbContextBuilder(string name = "default")
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebApiCoreSeedContext>();
            optionsBuilder.UseInMemoryDatabase(name);
            return optionsBuilder;
        }
        #endregion
    }
}
