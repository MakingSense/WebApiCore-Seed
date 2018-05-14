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
        #region Fields

        private Mock<IUserService> _userService;

        private UserController _classUnderTest;

        #endregion

        #region Constructors

        public UserControllerTest()
        {
            _userService = new Mock<IUserService>();
            _classUnderTest = new UserController(_userService.Object);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public async void GetAll_ShouldReturnAListOfUsers_WhenHasAtLeastOneUser()
        {
            var id = Guid.NewGuid();
            var users = new List<User>()
            {
                GetADefaultUser(id)
            };

            _userService.Setup(a => a.GetAsync()).Returns(Task.FromResult(users));

            var result = await _classUnderTest.GetAll();

            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Contains(okResult.Value as List<User>, a => a.Id == id);
            _userService.VerifyAll();
        }

        #endregion

        #region Get tests

        [Fact]
        public async void Get_ShouldReturnAnUser_WhenIdExist()
        {
            var id = Guid.NewGuid();
            var user = GetADefaultUser(id);

            _userService.Setup(a => a.GetByIdAsync(It.Is<Guid>(g => g == id)))
                .Returns(Task.FromResult(user));

            var result = await _classUnderTest.Get(id);

            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.True((okResult.Value as User).Id == id);
            _userService.VerifyAll();
        }

        [Fact]
        public async void Get_ShouldReturnNotFound_WhenIdNotExist()
        {
            var id = Guid.NewGuid();
            _userService.Setup(a => a.GetByIdAsync(It.Is<Guid>(g => g == id)))
                .Returns(Task.FromResult<User>(null));

            var result = await _classUnderTest.Get(id);

            Assert.IsType<NotFoundResult>(result);
            _userService.VerifyAll();
        }

        #endregion

        #region Create Tests

        [Fact]
        public async void Create_ShouldReturnNoContent_WhenUserIsOk()
        {
            var userDto = GetADefaultUserDto();

            _userService.Setup(a => a.CreateAsync(It.Is<User>(u =>
                  u.Email == userDto.Email &&
                  u.FirstName == userDto.FirstName &&
                  u.LastName == userDto.LastName &&
                  u.UserName == userDto.UserName)))
                  .Returns(Task.FromResult(1));

            var result = await _classUnderTest.Create(userDto);

            Assert.IsType<NoContentResult>(result);
            _userService.VerifyAll();
        }

        [Fact]
        public async void Create_ShouldReturnBadRequest_WhenInputIsNull()
        {
            var result = await _classUnderTest.Create(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void Create_ShouldReturnNotFound_WhenNoUserWasNotCreated()
        {
            var userDto = GetADefaultUserDto();

            _userService.Setup(a => a.CreateAsync(It.Is<User>(
                u =>
                    u.Email == userDto.Email &&
                    u.FirstName == userDto.FirstName &&
                    u.LastName == userDto.LastName &&
                    u.UserName == userDto.UserName)))
                .Returns(Task.FromResult(0));

            var result = await _classUnderTest.Create(userDto);

            Assert.IsType<NotFoundResult>(result);
            _userService.VerifyAll();
        }

        #endregion

        #region Update Tests

        [Fact]
        public async void Update_ShouldReturnNoContent_WhenUserIsOk()
        {
            var id = Guid.NewGuid();
            var userDto = GetADefaultUserDto();

            _userService.Setup(a => a.UpdateAsync(It.Is<User>(
                u =>
                    u.Id == id &&
                    u.Email == userDto.Email &&
                    u.FirstName == userDto.FirstName &&
                    u.LastName == userDto.LastName &&
                    u.UserName == userDto.UserName)))
                .Returns(Task.FromResult(1));

            var result = await _classUnderTest.Update(id, userDto);

            Assert.IsType<NoContentResult>(result);
            _userService.VerifyAll();
        }

        [Fact]
        public async void Update_ShouldReturnBadRequest_WhenUserIsNull()
        {
            var id = Guid.NewGuid();

            var result = await _classUnderTest.Update(id, null);

            Assert.IsType<BadRequestResult>(result);
            _userService.VerifyAll();
        }

        [Fact]
        public async void Update_ShouldReturnNotFound_WhenUserNotExist()
        {
            var id = Guid.NewGuid();
            var userDto = GetADefaultUserDto();

            _userService.Setup(a => a.UpdateAsync(It.Is<User>(
                u =>
                    u.Id == id &&
                    u.Email == userDto.Email &&
                    u.FirstName == userDto.FirstName &&
                    u.LastName == userDto.LastName &&
                    u.UserName == userDto.UserName)))
                .Returns(Task.FromResult(0));

            var result = await _classUnderTest.Update(id, userDto);

            Assert.IsType<NotFoundResult>(result);
            _userService.VerifyAll();
        }
        #endregion

        #region Delete Tests

        [Fact]
        public async void Delete_WhenIdExist_ShouldReturnNoContent()
        {
            var id = Guid.NewGuid();

            _userService.Setup(a => a.DeleteByIdAsync(It.Is<Guid>(g => g == id)))
                .Returns(Task.FromResult(1));

            var result = await _classUnderTest.Delete(id);

            Assert.IsType<NoContentResult>(result);
            _userService.VerifyAll();
        }

        [Fact]
        public async void Delete_WhenIdNotExist_ShouldReturnNotFound()
        {
            var id = Guid.NewGuid();

            _userService.Setup(a => a.DeleteByIdAsync(It.Is<Guid>(g => g == id)))
                .Returns(Task.FromResult(0));

            var result = await _classUnderTest.Delete(id);

            Assert.IsType<NotFoundResult>(result);
            _userService.VerifyAll();
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
