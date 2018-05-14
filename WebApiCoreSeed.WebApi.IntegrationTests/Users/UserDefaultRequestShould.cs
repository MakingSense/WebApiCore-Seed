using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiCoreSeed.Data.Models;
using Xunit;

namespace WebApiCoreSeed.WebApi.IntegrationTests.Users
{
    public class UserDefaultRequestShould
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public UserDefaultRequestShould()
        {
            var dir = Directory.GetCurrentDirectory();
            var file = new FileInfo(dir.ToString() + "/appsettings.json");
            if (!file.Exists)
                file.Create();
            var appjson = file.AppendText();
            appjson.Write("{\"Logging\": {\"IncludeScopes\": false,\"LogLevel\": {\"Default\": \"Warning\"}}}");
            appjson.Close();

            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task ReturnAListOfUsers_WhenGet()
        {
            var request = "/Users";

            var result = await _client.GetAsync(request);
            result.EnsureSuccessStatusCode();

            var users = JsonConvert.DeserializeObject<List<User>>(await result.Content.ReadAsStringAsync());
            Assert.NotNull(users);
        }
    }
}