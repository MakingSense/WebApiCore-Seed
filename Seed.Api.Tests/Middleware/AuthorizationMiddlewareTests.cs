using Microsoft.AspNetCore.Http;
using Moq;
using Seed.Api.Middleware;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Seed.Api.Tests.Middleware
{
    public class AuthorizationMiddlewareTests 
    {
        [Fact]
        public async void Invoke_ShouldInvokeNext_WhenUserIsValid()
        {
            var httpContext = new Mock<HttpContext>();
            var user = GetAnUser(new List<Claim>
            {
                new Claim("UserStatus", "Alive")
            });

            RequestDelegate next = (a) =>
            {
                return Task.Run(() => { });
            };

            httpContext.Setup(a => a.User).Returns(user);
            
            var classUnderTest = new AuthorizationMiddleware(next);
            await classUnderTest.Invoke(httpContext.Object);
            
            httpContext.Verify(a => a.Response, Times.Never);
        }

        [Fact]
        public async void Invoke_ShouldAssambleUnauthorized_WhenUserIsBlocked()
        {
            var httpContext = new Mock<HttpContext>();
            var httpResponse = new Mock<HttpResponse>();
            var user = GetAnUser(new List<Claim>
            {
                new Claim("UserStatus", "Blocked")
            });
            httpResponse.Setup(a => a.Body).Returns(new MemoryStream());
            httpContext.Setup(a => a.Response).Returns(httpResponse.Object);
            httpContext.Setup(a => a.User).Returns(user);

            var classUnderTest = new AuthorizationMiddleware(null);
            await classUnderTest.Invoke(httpContext.Object);

            httpResponse.VerifySet(a => 
                a.StatusCode = It.Is<int>(s => s == 401), 
                Times.Once);

            httpResponse.VerifySet(a =>
                a.ContentType = It.Is<string>(s => s.ToLower() == "application/json"),
                Times.Once);
        }

        private ClaimsPrincipal GetAnUser(IEnumerable<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, "testType");
            return new ClaimsPrincipal(identity);
        }
    }
}