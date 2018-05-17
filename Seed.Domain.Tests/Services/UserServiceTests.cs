using Microsoft.EntityFrameworkCore;
using Moq;
using Seed.Data.EF;
using Seed.Data.Models;
using Seed.Domain.Services;
using Seed.Domain.Services.Interfaces;
using System;
using System.Threading;
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
            var context = new Mock<WebApiCoreSeedContext>();
            var users = new Mock<DbSet<User>>();
            var createdUser = GetADefaultUser();
            context.Setup(x => x.Users).Returns(users.Object);
            users.Setup(x => x.FindAsync(It.Is<Guid>(y => y == createdUser.Id)))
                 .ReturnsAsync(createdUser)
                 .Verifiable();
            var userService = new UserService(context.Object);

            // Act
            var retrievedUser = await userService.GetByIdAsync(createdUser.Id);

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(createdUser.UserName, retrievedUser.UserName);
            Assert.Equal(createdUser.Email, retrievedUser.Email);
            Assert.Equal(createdUser.FirstName, retrievedUser.FirstName);
            Assert.Equal(createdUser.LastName, retrievedUser.LastName);
            context.VerifyAll();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull()
        {
            // Arrange
            var context = new Mock<WebApiCoreSeedContext>();
            var users = new Mock<DbSet<User>>();
            var id = Guid.NewGuid();
            User user = null;
            context.Setup(x => x.Users)
                   .Returns(users.Object);
            users.Setup(x => x.FindAsync(It.Is<Guid>(y => y == id)))
                 .ReturnsAsync(user)
                 .Verifiable();
            var userService = new UserService(context.Object);

            //Act
            var retrievedUser = await userService.GetByIdAsync(id);

            // Assert
            Assert.Null(retrievedUser);
            context.VerifyAll();
        }

        #endregion

        #region Detele Tests

        [Fact]
        public async void Delete_ShouldDeleteUser()
        {
            // Arrange
            var context = new Mock<WebApiCoreSeedContext>();
            var users = new Mock<DbSet<User>>();
            var createdUser = GetADefaultUser();
            context.Setup(x => x.Users)
                   .Returns(users.Object);
            users.Setup(x => x.FindAsync(It.Is<Guid>(y => y == createdUser.Id)))
                 .ReturnsAsync(createdUser)
                 .Verifiable();
            users.Setup(x => x.Remove(It.Is<User>(y => y.Id == createdUser.Id)))
                 .Verifiable();
            context.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);
            var userService = new UserService(context.Object);

            // Act
            var affectedRows = await userService.DeleteByIdAsync(createdUser.Id);

            // Assert
            Assert.True(affectedRows > 0);
            users.Verify(x => x.Remove(It.Is<User>(y => y.Id == createdUser.Id)), Times.Once);
            context.VerifyAll();
        }

        [Fact]
        public async void Delete_UserNotFound()
        {
            // Arrange
            var context = new Mock<WebApiCoreSeedContext>();
            var users = new Mock<DbSet<User>>();
            var id = Guid.NewGuid();
            User user = null;
            context.Setup(x => x.Users)
                   .Returns(users.Object);
            users.Setup(x => x.FindAsync(It.Is<Guid>(y => y == id)))
                 .ReturnsAsync(user)
                 .Verifiable();
            var userService = new UserService(context.Object);

            //Act
            var affectedRows = await userService.DeleteByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(0, affectedRows);
            users.Verify(x => x.Remove(It.Is<User>(y => y.Id == id)), Times.Never);
            context.VerifyAll();
        }

        #endregion

        #region Update Tests

        [Fact]
        public async void Update_ShouldUpdateIfUserExists()
        {
            // Arrange
            var context = new Mock<WebApiCoreSeedContext>();
            var users = new Mock<DbSet<User>>();
            var user = GetADefaultUser();
            context.Setup(x => x.Users)
                   .Returns(users.Object);
            users.Setup(x => x.FindAsync(It.Is<Guid>(y => y == user.Id)))
                 .ReturnsAsync(user)
                 .Verifiable();
            context.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);
            var userService = new UserService(context.Object);

            // Act
            int affectedRows = await userService.UpdateAsync(user);

            // Assert
            Assert.True(affectedRows > 0);
            context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            context.VerifyAll();
        }

        [Fact]
        public async void Update_ShouldReturnZeroIfUserNotExists()
        {
            // Arrange
            var context = new Mock<WebApiCoreSeedContext>();
            var users = new Mock<DbSet<User>>();
            var createdUser = GetADefaultUser();
            context.Setup(x => x.Users)
                   .Returns(users.Object);

            var id = Guid.NewGuid();
            users.Setup(a => a.FindAsync(It.Is<Guid>(g => g == id), It.IsAny<CancellationToken>()))
                 .Returns<User>(null);
            context.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);
            var userService = new UserService(context.Object);

            // Act
            var affectedRows = await userService.UpdateAsync(createdUser);

            // Assert
            Assert.Equal(0, affectedRows);
            context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async void Create_ShoulReturnOneIfCreated()
        {
            // Arrange
            var context = new Mock<WebApiCoreSeedContext>();
            var users = new Mock<DbSet<User>>();
            var createdUser = GetADefaultUser();
            context.Setup(x => x.Users)
                   .Returns(users.Object);
            users.Setup(x => x.Add(It.Is<User>(y => y.Id == createdUser.Id)))
                 .Verifiable();
            context.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);
            var userService = new UserService(context.Object);
            // Act
            var affectedRows = await userService.CreateAsync(createdUser);
            // Assert
            Assert.Equal(1, affectedRows);
            context.VerifyAll();
            users.Verify(x => x.Add(It.Is<User>(y => y.Id == createdUser.Id)), Times.Once);
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

        #endregion
    }
}
