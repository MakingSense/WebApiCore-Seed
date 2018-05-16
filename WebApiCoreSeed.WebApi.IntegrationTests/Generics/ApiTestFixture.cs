using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using WebApiCoreSeed.Data.EF;
using WebApiCoreSeed.WebApi.IntegrationTests.Fake;

namespace WebApiCoreSeed.WebApi.IntegrationTests.Generics
{
    public class ApiTestFixture
    {
        public ApiTestFixture()
        {
            FileFaker.Fake();

            var builder = new WebHostBuilder()
                .UseEnvironment("development")
                .UseStartup<Startup>();

            var server = new TestServer(builder);
            Client = server.CreateClient();
            DbContextOptions = server.Host.Services
                .GetService(typeof(DbContextOptions<WebApiCoreSeedContext>)) as DbContextOptions<WebApiCoreSeedContext>;
        }  

        public HttpClient Client { get; }

        public DbContextOptions<WebApiCoreSeedContext> DbContextOptions { get; }
    }
}