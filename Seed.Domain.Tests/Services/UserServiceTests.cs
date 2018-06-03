using Seed.Data.EF;
using Seed.Data.Models;
using Seed.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Seed.Domain.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task GetByIdAsync_ReturnsUser()
        {
            // Arrange
            var dbContextOptions = DbContextHelper.GetOptions<WebApiCoreSeedContext>();
            var user = GetSampleUser();

            using (var dbContext = new WebApiCoreSeedContext(dbContextOptions))
            {
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }

            // Act
            User result;
            using (var dbContext = new WebApiCoreSeedContext(dbContextOptions))
            {
                var userService = new UserService(dbContext);
                result = await userService.GetByIdAsync(user.Id);
            }

            // Assert
            AssertUser(user, result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            var dbContextOptions = DbContextHelper.GetOptions<WebApiCoreSeedContext>();

            // Act
            User result;
            using (var dbContext = new WebApiCoreSeedContext(dbContextOptions))
            {
                var userService = new UserService(dbContext);
                result = await userService.GetByIdAsync(Guid.NewGuid());
            }

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ReturnsUsers()
        {
            // Arrange
            var dbContextOptions = DbContextHelper.GetOptions<WebApiCoreSeedContext>();
            var users = new List<User>
            {
                GetSampleUser(),
                GetSampleUser(),
                GetSampleUser()
            };

            using (var dbContext = new WebApiCoreSeedContext(dbContextOptions))
            {
                await dbContext.Users.AddRangeAsync(users);
                await dbContext.SaveChangesAsync();
            }

            // Act
            List<User> result;
            using (var dbContext = new WebApiCoreSeedContext(dbContextOptions))
            {
                var userService = new UserService(dbContext);
                result = await userService.GetAsync();
            }

            // Assert
            Assert.Equal(users.Count, result.Count);
            foreach (var user in result)
            {
                var expected = users.SingleOrDefault(u => u.Id == user.Id);
                AssertUser(expected, user);
            }
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedUser()
        {
            // Arrange
            var dbContextOptions = DbContextHelper.GetOptions<WebApiCoreSeedContext>();
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@test.com",
                UserName = "johndoe"
            };

            // Act
            User result;
            using (var dbContext = new WebApiCoreSeedContext(dbContextOptions))
            {
                var userService = new UserService(dbContext);
                result = await userService.CreateAsync(user);
            }

            // Assert
            Assert.True(result.CreatedOn > DateTime.Now.AddMinutes(-1));
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.UserName, result.UserName);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsUpdatedUser()
        {
            // Arrange
            var dbContextOptions = DbContextHelper.GetOptions<WebApiCoreSeedContext>();
            var user = GetSampleUser();

            using (var dbContext = new WebApiCoreSeedContext(dbContextOptions))
            {
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }

            var toUpdate = new User
            {
                Id = user.Id,
                FirstName = $"Updated {user.FirstName}",
                LastName = $"Updated {user.LastName}",
                Email = "new-email@test.com",
                UserName = $"updated-{user.UserName}"
            };

            // Act
            User result;
            using (var dbContext = new WebApiCoreSeedContext(dbContextOptions))
            {
                var userService = new UserService(dbContext);
                result = await userService.UpdateAsync(toUpdate);
            }

            // Assert
            Assert.True(result.UpdatedOn > DateTime.Now.AddMinutes(-1));
            Assert.Equal(toUpdate.Email, result.Email);
            Assert.Equal(toUpdate.FirstName, result.FirstName);
            Assert.Equal(toUpdate.LastName, result.LastName);
            Assert.Equal(toUpdate.UserName, result.UserName);
        }

        [Fact]
        public async Task DeleteAsync_WhenUserExists_ReturnsTrue()
        {
            // Arrange
            var dbContextOptions = DbContextHelper.GetOptions<WebApiCoreSeedContext>();
            var user = GetSampleUser();

            using (var dbContext = new WebApiCoreSeedContext(dbContextOptions))
            {
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }

            // Act
            bool result;
            User expectedUser;
            using (var dbContext = new WebApiCoreSeedContext(dbContextOptions))
            {
                var userService = new UserService(dbContext);
                result = await userService.DeleteAsync(user.Id);
                expectedUser = dbContext.Users.SingleOrDefault(u => u.Id == user.Id);
            }

            // Assert
            Assert.True(result);
            Assert.Null(expectedUser);
        }

        [Fact]
        public async Task DeleteAsync_WhenUserNotExists_ReturnsFalse()
        {
            // Arrange
            var dbContextOptions = DbContextHelper.GetOptions<WebApiCoreSeedContext>();
            
            // Act
            bool result;
            using (var dbContext = new WebApiCoreSeedContext(dbContextOptions))
            {
                var userService = new UserService(dbContext);
                result = await userService.DeleteAsync(Guid.NewGuid());
            }

            // Assert
            Assert.False(result);
        }

        private void AssertUser(User expected, User actual)
        {
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
            Assert.Equal(expected.UserName, actual.UserName);
            Assert.Equal(expected.Email, actual.Email);
        }

        private User GetSampleUser(Guid? id = null)
        {
            return new User
            {
                Id = id ?? Guid.NewGuid(),
                Email = "johndoe@gmail.com",
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                CreatedOn = DateTime.Now
            };
        }
    }
}
