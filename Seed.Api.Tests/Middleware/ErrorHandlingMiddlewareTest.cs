﻿using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using WebApiCoreSeed.WebApi.Middleware;
using Xunit;

namespace WebApiCoreSeed.WebApi.Tests.Middleware
{
    public class ErrorHandlingMiddlewareTest
    {
        [Fact]
        public async void Invoke_ShouldDoNothing_WhenNextThrowsNoExceptions()
        {
            var httpContext = new Mock<HttpContext>();
            var httpResponse = new Mock<HttpResponse>();

            httpContext.Setup(a => a.Response).Returns(httpResponse.Object);

            RequestDelegate next = (a) =>
            {
                return Task.Run(() => { });
            };

            var classUnderTest = new ErrorHandlingMiddleware(next);

            await classUnderTest.Invoke(httpContext.Object);

            VerifyHttpResponse(httpResponse, It.IsAny<string>(), It.IsAny<int>(), Times.Never());
        }

        [Theory]
        [ClassData(typeof(ErrorHandlingMiddlewareTestData))]
        public async void Invoke_ShouldAssembleAnErrorResponse_WhenNextThrowsAnException(Exception ex, int statusCode)
        {
            var httpContext = new Mock<HttpContext>();
            var httpResponse = new Mock<HttpResponse>();

            httpContext.Setup(a => a.Response).Returns(httpResponse.Object);
            httpResponse.Setup(a => a.Body).Returns(new MemoryStream());

            RequestDelegate next = (a) =>
            {
                return Task.FromException(ex);
            };

            var classUnderTest = new ErrorHandlingMiddleware(next);

            await classUnderTest.Invoke(httpContext.Object);

            VerifyHttpResponse(httpResponse, "application/json", statusCode, Times.Once());
        }

        private void VerifyHttpResponse(Mock<HttpResponse> httpResponse, string contentType, int statusCode, Times times)
        {
            httpResponse.VerifySet(a =>
                a.ContentType = contentType,
                times);

            httpResponse.VerifySet(a =>
                a.StatusCode = statusCode,
                times);
        }
    }
}
