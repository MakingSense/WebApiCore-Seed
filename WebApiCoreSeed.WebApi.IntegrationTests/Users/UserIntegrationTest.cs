using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiCoreSeed.Data.Models;
using WebApiCoreSeed.WebApi.IntegrationTests.Fake;
using Xunit;

namespace WebApiCoreSeed.WebApi.IntegrationTests.Users
{
    public class UserIntegrationTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public UserIntegrationTest()
        {
            SwaggerXmlFaker.Fake();

            var builder = new WebHostBuilder()
                .UseEnvironment("development")
                .UseStartup<Startup>();

            _server = new TestServer(builder);
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task Get_ShouldReturnAListOfUsers_WhenNoParameters()
        {
            var request = "api/users";

            var result = await _client.GetAsync(request);
            result.EnsureSuccessStatusCode();

            var users = JsonConvert.DeserializeObject<List<User>>(await result.Content.ReadAsStringAsync());
            Assert.NotNull(users);
        }

        [Fact]
        public async Task Get_ShouldReturnAnUser_WhenNoParameters()
        {
            var request = "api/users";

            var result = await _client.GetAsync(request);
            result.EnsureSuccessStatusCode();

            var users = JsonConvert.DeserializeObject<List<User>>(await result.Content.ReadAsStringAsync());

            var guid = users.First().Id;
            request += "/" + guid.ToString();

            result = await _client.GetAsync(request);

            result.EnsureSuccessStatusCode();

            var user = JsonConvert.DeserializeObject<User>(await result.Content.ReadAsStringAsync());

            Assert.Equal(guid, user.Id);
        }
    }
}