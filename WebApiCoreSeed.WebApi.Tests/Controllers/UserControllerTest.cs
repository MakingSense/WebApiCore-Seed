using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiCoreSeed.Data.Models;
using WebApiCoreSeed.Domain.Services.Interfaces;
using WebApiCoreSeed.WebApi.Controllers;
using WebApiCoreSeed.WebApi.Models;
using Xunit;

namespace WebApiCoreSeed.WebApi.Tests.Controllers
{
    public class UserControllerTest
    {
        #region GetAll Tests

        [Fact]
        public async void GetAll_ShouldReturnAListOfUsers_WhenHasAtLeastOneUser()
        {
            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var id = Guid.NewGuid();
            var users = new List<User>()
            {
                GetADefaultUser(id)
            };

            userService.Setup(a => a.GetAsync()).ReturnsAsync(users);

            var result = await classUnderTest.GetAll();

            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Contains(okResult.Value as List<User>, a => a.Id == id);
            userService.VerifyAll();
        }

        #endregion

        #region Get tests

        [Fact]
        public async void Get_ShouldReturnAnUser_WhenIdExists()
        {
            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var id = Guid.NewGuid();
            var user = GetADefaultUser(id);

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
        public async void Create_ShouldReturnNoContent_WhenUserIsOk()
        {
            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var userDto = GetADefaultUserDto();

            userService.Setup(a => a.CreateAsync(It.Is<User>(u =>
                  u.Email == userDto.Email &&
                  u.FirstName == userDto.FirstName &&
                  u.LastName == userDto.LastName &&
                  u.UserName == userDto.UserName)))
                  .ReturnsAsync(1);

            var result = await classUnderTest.Create(userDto);

            Assert.IsType<NoContentResult>(result);
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

        [Fact]
        public async void Create_ShouldReturnNotFound_WhenNoUserWasNotCreated()
        {
            var userService = new Mock<IUserService>();
            var classUnderTest = new UserController(userService.Object);

            var userDto = GetADefaultUserDto();

            userService.Setup(a => a.CreateAsync(It.Is<User>(
                u =>
                    u.Email == userDto.Email &&
                    u.FirstName == userDto.FirstName &&
                    u.LastName == userDto.LastName &&
                    u.UserName == userDto.UserName)))
                .ReturnsAsync(0);

            var result = await classUnderTest.Create(userDto);

            Assert.IsType<NotFoundResult>(result);
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

        private static User GetADefaultUser(Guid? id = null)
        {
            var sanitizedId = id ?? Guid.NewGuid();

            return new User
            {
                Id = sanitizedId,
                Email = "fn@ms.com",
                FirstName = "firstName",
                CreatedBy = "someUser",
                CreatedOn = DateTime.Now,
                LastName = "someUser",
                UserName = "fn"
            };
        }

        private static UserDto GetADefaultUserDto()
        {
            return new UserDto
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
