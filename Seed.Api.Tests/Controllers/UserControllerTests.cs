using Microsoft.AspNetCore.Mvc;
using Moq;
using Seed.Api.Controllers;
using Seed.Api.Models;
using Seed.Data.Models;
using Seed.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Seed.Api.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userService;

        public UserControllerTests()
        {
            _userService = new Mock<IUserService>();
        }

        [Fact]
        public async void GetAll_ReturnsOk()
        {
            // Arrange
            var controller = new UserController(_userService.Object);
            var sampleUsers = new List<User>()
            {
                GetSampleUser(),
                GetSampleUser(),
                GetSampleUser()
            };

            _userService.Setup(mock => mock.GetAsync()).ReturnsAsync(sampleUsers);

            // Act
            var result = await controller.GetAll();

            // Assert
            _userService.Verify(mock => mock.GetAsync(), Times.Once);

            Assert.IsType<OkObjectResult>(result);
            var okObjectResult = result as OkObjectResult;
            var users = okObjectResult.Value as List<UserDto>;

            Assert.Equal(sampleUsers.Count, users.Count);
            foreach (var user in users)
            {
                var expected = sampleUsers.SingleOrDefault(u => u.Id == user.Id);
                Assert.Equal(expected.FirstName, user.FirstName);
                Assert.Equal(expected.LastName, user.LastName);
                Assert.Equal(expected.UserName, user.UserName);
                Assert.Equal(expected.Email, user.Email);
            }
        }

        [Fact]
        public async void Get_ReturnsOk()
        {
            // Arrange
            var controller = new UserController(_userService.Object);
            var sampleUser = GetSampleUser();
            _userService.Setup(mock => mock.GetByIdAsync(sampleUser.Id)).ReturnsAsync(sampleUser);

            // Act
            var result = await controller.Get(sampleUser.Id);

            // Assert
            _userService.Verify(mock => mock.GetByIdAsync(sampleUser.Id), Times.Once);

            Assert.IsType<OkObjectResult>(result);
            var okObjectResult = result as OkObjectResult;
            var user = okObjectResult.Value as UserDto;

            Assert.Equal(sampleUser.Id, user.Id);
            Assert.Equal(sampleUser.FirstName, user.FirstName);
            Assert.Equal(sampleUser.LastName, user.LastName);
            Assert.Equal(sampleUser.UserName, user.UserName);
            Assert.Equal(sampleUser.Email, user.Email);
        }

        [Fact]
        public async void Get_ReturnsNotFound_WhenUserNotExists()
        {
            // Arrange
            var controller = new UserController(_userService.Object);
            var userId = Guid.NewGuid();
            _userService.Setup(mock => mock.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await controller.Get(userId);

            // Assert
            _userService.Verify(mock => mock.GetByIdAsync(userId), Times.Once);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Create_ReturnsCreated()
        {
            // Arrange
            var controller = new UserController(_userService.Object);
            var expected = GetSampleUser();
            var sampleUser = new InputUserDto
            {
                Email = expected.Email,
                FirstName = expected.FirstName,
                LastName = expected.LastName,
                UserName = expected.UserName
            };

            _userService.Setup(mock => mock.CreateAsync(It.IsAny<User>())).ReturnsAsync(expected);

            // Act
            var result = await controller.Create(sampleUser);

            // Assert
            _userService.Verify(mock => mock.CreateAsync(It.IsAny<User>()), Times.Once);

            Assert.IsType<CreatedAtActionResult>(result);
            var createdResult = result as CreatedAtActionResult;
            var user = createdResult.Value as UserDto;

            Assert.Equal(expected.Id, user.Id);
            Assert.Equal(expected.FirstName, user.FirstName);
            Assert.Equal(expected.LastName, user.LastName);
            Assert.Equal(expected.UserName, user.UserName);
            Assert.Equal(expected.Email, user.Email);
        }

        [Fact(Skip = "Validation attribute is not working")]
        public async void Create_ReturnsBadRequest_WhenInvalidInput()
        {
            // Arrange
            var controller = new UserController(_userService.Object);
            var user = new InputUserDto
            {
                Email = "invalid-address"
            };

            // Act
            var result = await controller.Create(user);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void Update_ReturnsNoContent()
        {
            // Arrange
            var controller = new UserController(_userService.Object);
            var expected = GetSampleUser();
            var sampleUser = new InputUserDto
            {
                Email = expected.Email,
                FirstName = expected.FirstName,
                LastName = expected.LastName,
                UserName = expected.UserName
            };

            _userService.Setup(mock => mock.UpdateAsync(It.IsAny<User>())).ReturnsAsync(expected);

            // Act
            var result = await controller.Update(expected.Id, sampleUser);

            // Assert
            _userService.Verify(mock => mock.UpdateAsync(It.IsAny<User>()), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact(Skip = "Validation attribute is not working")]
        public async void Update_ReturnsBadRequest()
        {
            // Arrange
            var controller = new UserController(_userService.Object);
            var user = new InputUserDto
            {
                Email = "invalid-address"
            };

            // Act
            var result = await controller.Update(Guid.NewGuid(), user);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void Update_ReturnsNotFound_WhenUserNotExists()
        {
            // Arrange
            var controller = new UserController(_userService.Object);
            var sampleUser = GetSampleInputUserDto();
            _userService.Setup(mock => mock.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User)null);

            // Act
            var result = await controller.Update(Guid.NewGuid(), sampleUser);

            // Assert
            _userService.Verify(mock => mock.UpdateAsync(It.IsAny<User>()), Times.Once);
            Assert.IsType<NotFoundResult>(result);
        }

        #region Delete Tests

        [Fact]
        public async void Delete_WhenIdExists_ShouldReturnNoContent()
        {
            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var id = Guid.NewGuid();

            userService.Setup(a => a.DeleteByIdAsync(It.Is<Guid>(g => g == id)))
                .ReturnsAsync(1);

            var result = await classUnderTest.Delete(id);

            Assert.IsType<NoContentResult>(result);
            userService.VerifyAll();
        }

        [Fact]
        public async void Delete_WhenIdNotExists_ShouldReturnNotFound()
        {
            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var id = Guid.NewGuid();

            userService.Setup(a => a.DeleteByIdAsync(It.Is<Guid>(g => g == id)))
                .ReturnsAsync(0);

            var result = await classUnderTest.Delete(id);

            Assert.IsType<NotFoundResult>(result);
            userService.VerifyAll();
        }

        #endregion

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

        private InputUserDto GetSampleInputUserDto()
        {
            return new InputUserDto
            {
                Email = "johndoe@gmail.com",
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe"
            };
        }
    }
}
