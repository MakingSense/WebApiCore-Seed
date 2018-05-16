namespace Seed.Infrastructure.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class MockedDelegatingHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage _mockedResponseMessage;

        public MockedDelegatingHandler(HttpResponseMessage mockedResponseMessage)
        {
            _mockedResponseMessage = mockedResponseMessage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return await Task.FromResult(_mockedResponseMessage);
        }
    }
}