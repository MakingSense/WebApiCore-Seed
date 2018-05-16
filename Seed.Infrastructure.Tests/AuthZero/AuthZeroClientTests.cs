using Auth0.AuthenticationApi.Models;
using Auth0.Core;
using Auth0.Core.Exceptions;
using Moq;
using Seed.Infrastructure.AuthZero;
using Seed.Infrastructure.RestClient;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Seed.Infrastructure.Tests.AuthZero
{
    public class AuthZeroClientTests
    {
        [Fact]
        public async Task ObtainManagementApiToken_ShouldReturnAccessToken_WhenCredentialsAreAccepted()
        {
            // Arrange
            var restClientMock = new Mock<IRestClient>();
            var mockedToken = new AccessToken
            {
                AccessToken = "testToken",
                ExpiresIn = 86400
            };

            var mockedResponse = new RestResponse<AccessToken, ApiError>(HttpStatusCode.Accepted, mockedToken);
            restClientMock.Setup(rc => rc.PostAsync<AccessToken, ApiError>("/oauth/token", It.IsAny<object>())).ReturnsAsync(mockedResponse);
            IAuthZeroClient authZeroClient = new AuthZeroClient(restClientMock.Object, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Act
            var tokenResult = await authZeroClient.ObtainManagementApiToken();

            // Assert
            Assert.True(tokenResult.IsSuccessResult);
            Assert.Equal(mockedToken.AccessToken, tokenResult.SuccessValue.AccessToken);
        }

        [Fact]
        public async Task ObtainManagementApiToken_ShouldReturnNull_WhenCredentialsAreNotAccepted()
        {
            // Arrange
            var restClientMock = new Mock<IRestClient>();
            var mockedError = new ApiError
            {
                ErrorCode = "SomeErrorCode",
                Message = "Some error message"
            };

            var mockedResponse = new ApiException(HttpStatusCode.Unauthorized, mockedError);
            restClientMock.Setup(rc => rc.PostAsync<AccessToken, ApiError>("/oauth/token", It.IsAny<object>())).ThrowsAsync(mockedResponse);
            IAuthZeroClient authZeroClient = new AuthZeroClient(restClientMock.Object, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            // Act
            var tokenResult = await authZeroClient.ObtainManagementApiToken();

            // Assert
            Assert.False(tokenResult.IsSuccessResult);
            Assert.Equal(mockedError.ErrorCode, tokenResult.ErrorValue.Code);
            Assert.Equal(mockedError.Message, tokenResult.ErrorValue.Description);
        }

        [Fact]
        public async Task ObtainManagementApiToken_ShouldAutomaticallyRenewToken_WhenExistingOneIsExpired()
        {
            // Arrange
            var restClientMock = new Mock<IRestClient>();
            var mockedResponses = new[]
            {
                new RestResponse<AccessToken, ApiError>(HttpStatusCode.Accepted, new AccessToken { AccessToken = "testToken1", ExpiresIn = 1 }),
                new RestResponse<AccessToken, ApiError>(HttpStatusCode.Accepted, new AccessToken { AccessToken = "testToken2", ExpiresIn = 2 }),
                new RestResponse<AccessToken, ApiError>(HttpStatusCode.Accepted, new AccessToken { AccessToken = "testToken3", ExpiresIn = 1 }),
                new RestResponse<AccessToken, ApiError>(HttpStatusCode.Accepted, new AccessToken { AccessToken = "testToken4", ExpiresIn = 4 })
            };

            restClientMock.SetupSequence(rc => rc.PostAsync<AccessToken, ApiError>("/oauth/token", It.IsAny<object>()))
                .ReturnsAsync(mockedResponses[0])
                .ReturnsAsync(mockedResponses[1])
                .ReturnsAsync(mockedResponses[2])
                .ReturnsAsync(mockedResponses[3]);

            IAuthZeroClient authZeroClient = new AuthZeroClient(restClientMock.Object, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 0);

            for (var i = 0; i < mockedResponses.Length - 1; i++)
            {
                // Act
                var token1 = await authZeroClient.ObtainManagementApiToken();
                var token2 = await authZeroClient.ObtainManagementApiToken(); // Should be the same token as above because it's still valid
                await Task.Delay(2500);
                var token3 = await authZeroClient.ObtainManagementApiToken(); // Should be a new token because the old one expired

                // Assert
                Assert.Equal(mockedResponses[i].SuccessValue.AccessToken, token1.SuccessValue.AccessToken);
                Assert.Equal(mockedResponses[i].SuccessValue.AccessToken, token2.SuccessValue.AccessToken);
                Assert.Equal(mockedResponses[i + 1].SuccessValue.AccessToken, token3.SuccessValue.AccessToken);
            }
        }

        [Fact]
        public async Task ObtainManagementApiToken_ShouldAutomaticallyRenewToken_BeforeExistingOneIsExpiredWhenEarlyRefreshIsConfigured()
        {
            // Arrange
            var restClientMock = new Mock<IRestClient>();
            var mockedResponses = new[]
            {
                new RestResponse<AccessToken, ApiError>(HttpStatusCode.Accepted, new AccessToken { AccessToken = "testToken1", ExpiresIn = 5 }),
                new RestResponse<AccessToken, ApiError>(HttpStatusCode.Accepted, new AccessToken { AccessToken = "testToken2", ExpiresIn = 5 }),
                new RestResponse<AccessToken, ApiError>(HttpStatusCode.Accepted, new AccessToken { AccessToken = "testToken3", ExpiresIn = 5 }),
                new RestResponse<AccessToken, ApiError>(HttpStatusCode.Accepted, new AccessToken { AccessToken = "testToken4", ExpiresIn = 5 })
            };

            restClientMock.SetupSequence(rc => rc.PostAsync<AccessToken, ApiError>("/oauth/token", It.IsAny<object>()))
                .ReturnsAsync(mockedResponses[0])
                .ReturnsAsync(mockedResponses[1])
                .ReturnsAsync(mockedResponses[2])
                .ReturnsAsync(mockedResponses[3]);

            var secondsForEarlyRefreshOfTokens = 3;
            IAuthZeroClient authZeroClient = new AuthZeroClient(restClientMock.Object, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), secondsForEarlyRefreshOfTokens);

            for (var i = 0; i < mockedResponses.Length - 1; i++)
            {
                // Act
                var token1 = await authZeroClient.ObtainManagementApiToken();
                var token2 = await authZeroClient.ObtainManagementApiToken(); // Should be the same token as above because it's still valid
                await Task.Delay(2500);
                var token3 = await authZeroClient.ObtainManagementApiToken(); // Should be new token because the old one is about to expire (based on secondsForEarlyRefreshOfTokens config)

                // Assert
                Assert.Equal(mockedResponses[i].SuccessValue.AccessToken, token1.SuccessValue.AccessToken);
                Assert.Equal(mockedResponses[i].SuccessValue.AccessToken, token2.SuccessValue.AccessToken);
                Assert.Equal(mockedResponses[i + 1].SuccessValue.AccessToken, token3.SuccessValue.AccessToken);
            }
        }
    }
}
