using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiCoreSeed.Infrastructure.Tests
{
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