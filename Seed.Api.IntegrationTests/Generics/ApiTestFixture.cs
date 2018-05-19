using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Seed.Api.IntegrationTests.Fake;
using Seed.Data.EF;
using System;
using System.Net.Http;

namespace Seed.Api.IntegrationTests.Generics
{
    public class ApiTestFixture : IDisposable
    {
        private readonly TestServer _server;

        public ApiTestFixture()
        {
            FileFaker.Fake();

            var builder = new WebHostBuilder()
                .UseEnvironment("development")
                .UseStartup<Startup>();

            _server = new TestServer(builder);

            Client = _server.CreateClient();
            DbContextOptions = _server.Host.Services.GetService(typeof(DbContextOptions<WebApiCoreSeedContext>)) as DbContextOptions<WebApiCoreSeedContext>;
        }

        public HttpClient Client { get; }

        public DbContextOptions<WebApiCoreSeedContext> DbContextOptions { get; }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}