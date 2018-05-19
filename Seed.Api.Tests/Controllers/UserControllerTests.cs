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
        public async void GetAll_ShouldReturnUsers()
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

        #region Get tests

        [Fact]
        public async void Get_ShouldReturnAUser_WhenIdExists()
        {
            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var id = Guid.NewGuid();
            var user = GetSampleUser(id);

            userService.Setup(a => a.GetByIdAsync(It.Is<Guid>(g => g == id)))
                .ReturnsAsync(user);

            var result = await classUnderTest.Get(id);

            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.True((okResult.Value as User).Id == id);
            userService.VerifyAll();
        }

        [Fact]
        public async void Get_ShouldReturnNotFound_WhenIdNotExists()
        {
            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var id = Guid.NewGuid();
            userService.Setup(a => a.GetByIdAsync(It.Is<Guid>(g => g == id)))
                .ReturnsAsync(null as User);

            var result = await classUnderTest.Get(id);

            Assert.IsType<NotFoundResult>(result);
            userService.VerifyAll();
        }

        #endregion

        #region Create Tests

        [Fact]
        public async void Create_ShouldReturnCreated_WhenUserIsOk()
        {
            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var userDto = GetADefaultUserDto();

            userService.Setup(a => a.CreateAsync(It.Is<User>(u =>
                  u.Email == userDto.Email &&
                  u.FirstName == userDto.FirstName &&
                  u.LastName == userDto.LastName &&
                  u.UserName == userDto.UserName)))
                  .ReturnsAsync(new User()
                  {
                      Id = Guid.NewGuid()
                  });

            var result = await classUnderTest.Create(userDto);

            Assert.IsType<CreatedAtActionResult>(result);
            userService.VerifyAll();
        }

        [Fact]
        public async void Create_ShouldReturnBadRequest_WhenInputIsNull()
        {
            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var result = await classUnderTest.Create(null);

            Assert.IsType<BadRequestResult>(result);
            userService.VerifyAll();
        }

        #endregion

        #region Update Tests

        [Fact]
        public async void Update_ShouldReturnNoContent_WhenUserIsOk()
        {
            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var id = Guid.NewGuid();
            var userDto = GetADefaultUserDto();

            userService.Setup(a => a.UpdateAsync(It.Is<User>(
                u =>
                    u.Id == id &&
                    u.Email == userDto.Email &&
                    u.FirstName == userDto.FirstName &&
                    u.LastName == userDto.LastName &&
                    u.UserName == userDto.UserName)))
                .ReturnsAsync(1);

            var result = await classUnderTest.Update(id, userDto);

            Assert.IsType<NoContentResult>(result);
            userService.VerifyAll();
        }

        [Fact]
        public async void Update_ShouldReturnBadRequest_WhenUserIsNull()
        {
            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var id = Guid.NewGuid();

            var result = await classUnderTest.Update(id, null);

            Assert.IsType<BadRequestResult>(result);
            userService.VerifyAll();
        }

        [Fact]
        public async void Update_ShouldReturnNotFound_WhenUserNotExists()
        {

            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var id = Guid.NewGuid();
            var userDto = GetADefaultUserDto();

            userService.Setup(a => a.UpdateAsync(It.Is<User>(
                u =>
                    u.Id == id &&
                    u.Email == userDto.Email &&
                    u.FirstName == userDto.FirstName &&
                    u.LastName == userDto.LastName &&
                    u.UserName == userDto.UserName)))
                .ReturnsAsync(0);

            var result = await classUnderTest.Update(id, userDto);

            Assert.IsType<NotFoundResult>(result);
            userService.VerifyAll();
        }
        #endregion

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

        #region Private Methods

        private User GetSampleUser(Guid? id = null)
        {
            return new User
            {
                Id = id ?? Guid.NewGuid(),
                Email = "johndoe@gmail.com",
                FirstName = "John",
                LastName = "Doe",
                UserName = "fn",
                CreatedOn = DateTime.Now
            };
        }

        private static InputUserDto GetADefaultUserDto()
        {
            return new InputUserDto
            {
                Email = "fn@ms.com",
                FirstName = "firstName",
                LastName = "someUser",
                UserName = "fn"
            };
        }

        #endregion
    }
}
