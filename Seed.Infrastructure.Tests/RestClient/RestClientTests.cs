namespace Seed.Infrastructure.Tests.RestClient
{
    using Seed.Infrastructure.RestClient;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class RestClientTests
    {
        private const string GenericErrorDescription = "Generic Error";

        private const string TestResourceJson =
            @"{
                ""Id"" : ""test"",
                ""code"" : ""5""
              }";

        private const string TestErrorJson =
            @"{
                ""ErrorCode"" : ""2"",
                ""ErrorDescription"" : """ + GenericErrorDescription + @"""
              }";

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("http:<>")]
        [InlineData("http:://test")]
        [InlineData("http:/test.com")]
        public void Constructor_ShouldThrowArgumentException_WhenBaseUrlIsInvalid(string baseUrl)
        {
            Assert.Throws<ArgumentException>(() => new RestClient(baseUrl, new HttpClient()));
        }

        [Theory]
        [InlineData("http://test.com", "")]
        [InlineData("http://test.com", null)]
        [InlineData("https://test.com", "")]
        [InlineData("https://test.com", null)]
        [InlineData("http://test.com", "ftp://a")]
        public async Task PostAsync_ShouldThrowArgumentException_WhenRelativeUrlIsInvalidAsync(string baseUrl, string relativeUrl)
        {
            // Arrange
            var restClient = new RestClient(baseUrl, new HttpClient());

            // Act
            Func<Task> testFunction = () => restClient.PostAsync<object, object>(relativeUrl, body: new { Test = "Test" });

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(testFunction);
        }

        [Fact]
        public async Task PostAsync_ShouldReturnResourceObject_WhenRequestIsSuccessful()
        {
            // Arrange
            var mockedResponseHandler = new MockedDelegatingHandler(new HttpResponseMessage
            {
                Content = new StringContent(TestResourceJson, Encoding.UTF8, "application/json"),
                StatusCode = HttpStatusCode.Created,
            });

            var restClient = new RestClient("http://test.com", new HttpClient(mockedResponseHandler));

            // Act
            var response = await restClient.PostAsync<TestResourceDto, ErrorDto>(relativeUrl: "/test", body: new { Test = "Test" });

            // Assert
            Assert.IsType<RestResponse<TestResourceDto, ErrorDto>>(response);
            Assert.IsType<TestResourceDto>(response.SuccessValue);
            Assert.Null(response.ErrorValue);
            Assert.Equal(HttpStatusCode.Created, response.HttpStatusCode);
            Assert.True(response.IsSuccessResult);
            Assert.Equal("test", response.SuccessValue.Id);
            Assert.Equal(5, response.SuccessValue.Code);
        }

        [Fact]
        public async Task PostAsync_ShouldReturnErrorObject_WhenRequestIsNotSuccessful()
        {
            // Arrange
            var mockedResponseHandler = new MockedDelegatingHandler(new HttpResponseMessage
            {
                Content = new StringContent(TestErrorJson, Encoding.UTF8, "application/json"),
                StatusCode = HttpStatusCode.BadRequest,
            });
            var restClient = new RestClient("http://test.com", new HttpClient(mockedResponseHandler));

            // Act
            var response = await restClient.PostAsync<object, ErrorDto>(relativeUrl: "/test", body: new { Test = "Test" });

            // Assert
            Assert.IsType<RestResponse<object, ErrorDto>>(response);
            Assert.Null(response.SuccessValue);
            Assert.IsType<ErrorDto>(response.ErrorValue);
            Assert.Equal(HttpStatusCode.BadRequest, response.HttpStatusCode);
            Assert.False(response.IsSuccessResult);
            Assert.Equal(2, response.ErrorValue.ErrorCode);
            Assert.Equal(GenericErrorDescription, response.ErrorValue.ErrorDescription);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnResourceObject_WhenRequestIsSuccessful()
        {
            // Arrange
            var mockedResponseHandler = new MockedDelegatingHandler(new HttpResponseMessage
            {
                Content = new StringContent(TestResourceJson, Encoding.UTF8, "application/json"),
                StatusCode = HttpStatusCode.Created,
            });

            var restClient = new RestClient("http://test.com", new HttpClient(mockedResponseHandler));

            // Act
            var response = await restClient.GetAsync<TestResourceDto, ErrorDto>(relativeUrl: "/test");

            // Assert
            Assert.IsType<RestResponse<TestResourceDto, ErrorDto>>(response);
            Assert.IsType<TestResourceDto>(response.SuccessValue);
            Assert.Null(response.ErrorValue);
            Assert.Equal(HttpStatusCode.Created, response.HttpStatusCode);
            Assert.True(response.IsSuccessResult);
            Assert.Equal("test", response.SuccessValue.Id);
            Assert.Equal(5, response.SuccessValue.Code);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnErrorObject_WhenRequestIsNotSuccessful()
        {
            // Arrange
            var mockedResponseHandler = new MockedDelegatingHandler(new HttpResponseMessage
            {
                Content = new StringContent(TestErrorJson, Encoding.UTF8, "application/json"),
                StatusCode = HttpStatusCode.BadRequest,
            });
            var restClient = new RestClient("http://test.com", new HttpClient(mockedResponseHandler));

            // Act
            var response = await restClient.GetAsync<TestResourceDto, ErrorDto>(relativeUrl: "/test");

            // Assert
            Assert.IsType<RestResponse<TestResourceDto, ErrorDto>>(response);
            Assert.Null(response.SuccessValue);
            Assert.IsType<ErrorDto>(response.ErrorValue);
            Assert.Equal(HttpStatusCode.BadRequest, response.HttpStatusCode);
            Assert.False(response.IsSuccessResult);
            Assert.Equal(2, response.ErrorValue.ErrorCode);
            Assert.Equal(GenericErrorDescription, response.ErrorValue.ErrorDescription);
        }
    }
}
