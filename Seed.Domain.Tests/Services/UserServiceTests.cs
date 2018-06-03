using Moq;
using Seed.Data.EF;
using Seed.Data.Models;
using Seed.Domain.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Seed.Domain.Tests
{
    public class UserServiceTests
    {
        #region Members

        private readonly Mock<WebApiCoreSeedContext> _context;

        #endregion

        #region Constructors

        public UserServiceTests()
        {
            _context = new Mock<WebApiCoreSeedContext>();
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var service = new UserService(_context.Object);
            var user = GetADefaultUser();
            _context.Setup(x => x.Users.FindAsync(It.Is<Guid>(y => y == user.Id)))
                 .ReturnsAsync(user)
                 .Verifiable();

            // Act
            var retrievedUser = await service.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.UserName, retrievedUser.UserName);
            Assert.Equal(user.Email, retrievedUser.Email);
            Assert.Equal(user.FirstName, retrievedUser.FirstName);
            Assert.Equal(user.LastName, retrievedUser.LastName);
            _context.Verify(c => c.Users.FindAsync(user.Id), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull()
        {
            // Arrange 
            var service = new UserService(_context.Object);
            var id = Guid.NewGuid();
            _context.Setup(x => x.Users.FindAsync(It.Is<Guid>(y => y == id)))
                 .ReturnsAsync((User)null)
                 .Verifiable();

            //Act
            var retrievedUser = await service.GetByIdAsync(id);

            // Assert
            Assert.Null(retrievedUser);
            _context.Verify(c => c.Users.FindAsync(id), Times.Once);
        }

        #endregion

        #region Detele Tests

        [Fact]
        public async void Delete_ShouldDeleteUser()
        {
            // Arrange
            var service = new UserService(_context.Object);
            var createdUser = GetADefaultUser();
            _context.Setup(x => x.Users.FindAsync(It.Is<Guid>(y => y == createdUser.Id)))
                 .ReturnsAsync(createdUser)
                 .Verifiable();
            _context.Setup(x => x.Users.Remove(It.Is<User>(y => y.Id == createdUser.Id)))
                 .Verifiable();
            _context.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);

            // Act
            var result = await service.DeleteAsync(createdUser.Id);

            // Assert
            Assert.True(result);
            _context.Verify(x => x.Users.Remove(It.Is<User>(y => y.Id == createdUser.Id)), Times.Once);
        }

        [Fact]
        public async void Delete_UserNotFound()
        {
            // Arrange  
            var service = new UserService(_context.Object);
            var id = Guid.NewGuid();
            _context.Setup(x => x.Users.FindAsync(It.Is<Guid>(y => y == id)))
                 .ReturnsAsync((User)null)
                 .Verifiable();

            //Act
            var result = await service.DeleteAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
            _context.Verify(x => x.Users.Remove(It.Is<User>(y => y.Id == id)), Times.Never);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async void Update_ShouldUpdateIfUserExists()
        {
            // Arrange
            var service = new UserService(_context.Object);
            var user = GetADefaultUser();
            _context.Setup(x => x.Users.FindAsync(It.Is<Guid>(y => y == user.Id)))
                 .ReturnsAsync(user)
                 .Verifiable();
            _context.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);

            // Act
            var result = await service.UpdateAsync(user);

            // Assert
            Assert.NotNull(result);
            _context.Verify(x => x.Users.FindAsync(It.Is<Guid>(y => y == user.Id)), Times.Once);
            _context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void Update_ReturnsNull_WhenUserNotExists()
        {
            // Arrange
            var service = new UserService(_context.Object);
            var createdUser = GetADefaultUser();
            _context.Setup(a => a.Users.FindAsync(It.Is<Guid>(g => g == createdUser.Id)))
                    .ReturnsAsync((User)null);
            _context.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

            // Act
            var result = await service.UpdateAsync(createdUser);

            // Assert
            Assert.Null(result);
            _context.Verify(x => x.Users.FindAsync(It.Is<Guid>(y => y == createdUser.Id)), Times.Once);
            _context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
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
