using Auth0.Core;
using Auth0.Core.Exceptions;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Clients;
using Auth0.ManagementApi.Models;
using Moq;
using Seed.Infrastructure.AuthZero;
using Seed.Infrastructure.Result;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Seed.Infrastructure.Tests.AuthZero
{
    public class AuthZeroServiceTests
    {
        [Fact]
        public async Task CreateUserAsync_ShouldReturnAnErrorMessage_WhenAuthZeroRequestFailed()
        {
            // Arrange
            var managementApiMock = new Mock<IManagementApiClient>();
            var usersClientMock = new Mock<IUsersClient>();
            managementApiMock.Setup(ma => ma.Users).Returns(usersClientMock.Object);
            var mockedApiError = new ApiError
            {
                ErrorCode = "auth0_idp_error",
                Message = "The user already exists."
            };

            managementApiMock
                .Setup(ma => ma.Users.CreateAsync(It.IsAny<UserCreateRequest>()))
                .ThrowsAsync(new ApiException(HttpStatusCode.BadRequest, mockedApiError));
            var authZeroClient = new Mock<IAuthZeroClient>();
            authZeroClient.Setup(azc => azc.GetManagementApiClient())
                .ReturnsAsync(new Result<IManagementApiClient, ErrorResult>(managementApiMock.Object));

            var authZeroService = new AuthZeroService(authZeroClient.Object);

            // Act
            var result = await authZeroService.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>());

            // Assert
            Assert.Null(result.SuccessValue);
            Assert.False(result.IsSuccessResult);
            Assert.NotNull(result.ErrorValue);
            Assert.Equal(mockedApiError.ErrorCode, result.ErrorValue.Code);
            Assert.Equal(mockedApiError.Message, result.ErrorValue.Description);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldReturnUserId_WhenAuhtZeroRequestSucceeded()
        {
            // Arrange
            var managementApiMock = new Mock<IManagementApiClient>();
            var usersClientMock = new Mock<IUsersClient>();
            managementApiMock.Setup(ma => ma.Users).Returns(usersClientMock.Object);
            var mockedUser = new User
            {
                UserId = "NewUserId"
            };

            managementApiMock.Setup(ma => ma.Users.CreateAsync(It.IsAny<UserCreateRequest>())).ReturnsAsync(mockedUser);
            var authZeroClient = new Mock<IAuthZeroClient>();
            authZeroClient.Setup(azc => azc.GetManagementApiClient())
                .ReturnsAsync(new Result<IManagementApiClient, ErrorResult>(managementApiMock.Object));
            var authZeroService = new AuthZeroService(authZeroClient.Object);

            // Act
            var result = await authZeroService.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>());

            // Assert
            Assert.Null(result.ErrorValue);
            Assert.True(result.IsSuccessResult);
            Assert.NotNull(result.SuccessValue);
            Assert.Equal(mockedUser.UserId, result.SuccessValue.UserId);
        }
    }
}
