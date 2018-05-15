using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiCoreSeed.Data.EF;
using WebApiCoreSeed.Data.Models;
using WebApiCoreSeed.WebApi.IntegrationTests.Fake;
using WebApiCoreSeed.WebApi.IntegrationTests.Users.TestData;
using WebApiCoreSeed.WebApi.Models;
using Xunit;

namespace WebApiCoreSeed.WebApi.IntegrationTests.Users
{
    public class UserIntegrationTest
    {
        private const string ResouceUri = "api/users/";

        private readonly TestServer _server;
        private readonly HttpClient _client;

        private readonly DbContextOptions<WebApiCoreSeedContext> _dbContextOptions;

        public UserIntegrationTest()
        {
            FileFaker.Fake();

            var builder = new WebHostBuilder()
                .UseEnvironment("development")
                .UseStartup<Startup>();

            _server = new TestServer(builder);
            _client = _server.CreateClient();
            _dbContextOptions = _server.Host.Services
                .GetService(typeof(DbContextOptions<WebApiCoreSeedContext>)) as DbContextOptions<WebApiCoreSeedContext>;
        }

        #region Get Tests

        [Fact]
        public async Task Get_ShouldReturnAListOfUsers_WhenNoParameters()
        {
            var result = await _client.GetAsync(ResouceUri);
            result.EnsureSuccessStatusCode();

            var users = JsonConvert.DeserializeObject<List<User>>(await result.Content.ReadAsStringAsync());
            Assert.NotNull(users);

            using (var dbcontext = CreateContext())
            {
                Assert.True(dbcontext.Users.Any(a => users.Any(s => a.Id == s.Id)));
            }
        }

        [Fact]
        public async Task Get_ShouldReturnAUser_WhenIdExists()
        {
            Guid guid;
            using (var dbcontext = CreateContext())
            {
                guid = dbcontext.Users.First().Id;
            }
            var request = ResouceUri + guid.ToString();

            var result = await _client.GetAsync(request);

            result.EnsureSuccessStatusCode();

            var user = JsonConvert.DeserializeObject<User>(await result.Content.ReadAsStringAsync());

            Assert.Equal(guid, user.Id);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenIdNotExists()
        {
            var guid = Guid.NewGuid();

            var request = ResouceUri + guid.ToString();

            var result = await _client.GetAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        #endregion

        #region Post Tests

        [Fact]
        public async Task Post_ShouldCreateAUser_WhenAUserIsGiven()
        {
            var user = new UserDto()
            {
                Email = "testpersonIntegration@email.com",
                FirstName = "testName",
                LastName = "testLastName",
                UserName = "testpersonIntegration"
            };

            var result = await _client.PostAsync(ResouceUri, CreateContent(user));

            result.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            using (var dbcontext = CreateContext())
            {
                Assert.NotNull(dbcontext.Users.FirstOrDefault(a => a.UserName == user.UserName));
            }
        }

        [Theory]
        [ClassData(typeof(UserBadRequestTestData))]
        public async Task Post_ShouldReturnBadRequestAndNoSave_WhenInputInvalid(object user)
        {
            int ammountOfUsers;
            using (var dbcontext = CreateContext())
            {
                ammountOfUsers = await dbcontext.Users.CountAsync();
            }

            var result = await _client.PostAsync(ResouceUri, CreateContent(user));

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            using (var dbcontext = CreateContext())
            {
                Assert.Equal(ammountOfUsers, await dbcontext.Users.CountAsync());
            }
        }

        #endregion

        #region Put Tests

        [Fact]
        public async Task Put_ShouldUpdateAUser_WhenAUserIsGiven()
        {
            User dbUser;
            using (var dbcontext = CreateContext())
            {
                dbUser = dbcontext.Users.FirstOrDefault();
            }

            var user = new UserDto()
            {
                Email = "testpersonIntegration@email.com",
                FirstName = "testName",
                LastName = "testLastName",
                UserName = "testpersonIntegration"
            };

            var requestUri = $"{ResouceUri}{dbUser.Id.ToString()}";

            var result = await _client.PutAsync(requestUri, CreateContent(user));
            result.EnsureSuccessStatusCode();

            User afterPutUser;

            using (var dbcontext = CreateContext())
            {
                afterPutUser = await dbcontext.Users.FindAsync(dbUser.Id);
            }

            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            Assert.Equal(user.UserName, afterPutUser.UserName);
            Assert.Equal(user.FirstName, afterPutUser.FirstName);
            Assert.Equal(user.LastName, afterPutUser.LastName);
        }

        [Fact]
        public async Task Put_ShouldReturnNotFound_WhenIdNotExists()
        {
            var id = Guid.NewGuid();

            var user = new UserDto()
            {
                Email = "testpersonIntegration@email.com",
                FirstName = "testName",
                LastName = "testLastName",
                UserName = "testpersonIntegration"
            };

            var requestUri = $"{ResouceUri}{id.ToString()}";

            var result = await _client.PutAsync(requestUri, CreateContent(user));

            User afterPutUser;

            using (var dbcontext = CreateContext())
            {
                afterPutUser = await dbcontext.Users.FindAsync(id);
            }

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Null(afterPutUser);
        }

        [Theory]
        [ClassData(typeof(UserBadRequestTestData))]
        public async Task Put_ShouldReturnBadRequestAndMakeNoChanges_WhenInputIsInvalid(object user)
        {
            User dbUser;
            using (var dbcontext = CreateContext())
            {
                dbUser = await dbcontext.Users.FirstOrDefaultAsync();
            }

            var requestUri = $"{ResouceUri}{dbUser.Id.ToString()}";

            var result = await _client.PutAsync(requestUri, CreateContent(user));

            User afterPutUser;

            using (var dbcontext = CreateContext())
            {
                afterPutUser = await dbcontext.Users.FindAsync(dbUser.Id);
            }

            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(dbUser.UserName, afterPutUser.UserName);
            Assert.Equal(dbUser.FirstName, afterPutUser.FirstName);
            Assert.Equal(dbUser.LastName, afterPutUser.LastName);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ShouldDeleteAUser_WhenIdExists()
        {
            Guid userId;
            using (var dbContext = CreateContext())
            {
                userId = (await dbContext.Users.FirstAsync()).Id;
            }

            var result = await _client.DeleteAsync($"{ResouceUri}{userId.ToString()}");

            result.EnsureSuccessStatusCode();

            User user;
            using (var dbContext = CreateContext())
            {
                user = await dbContext.Users.FindAsync(userId);
            }

            Assert.Null(user);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFoundAndMakeNoChanges_WhenIdNotExists()
        {
            var userId = Guid.NewGuid();
            int userCount;
            using (var dbContext = CreateContext())
            {
                userCount = await dbContext.Users.CountAsync();
            }

            var result = await _client.DeleteAsync($"{ResouceUri}{userId.ToString()}");

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

            using (var dbContext = CreateContext())
            {
                Assert.Equal(userCount, await dbContext.Users.CountAsync());
            }
        }

        #endregion

        private HttpContent CreateContent(object @object) =>
            new StringContent(JsonConvert.SerializeObject(@object), Encoding.UTF8, "application/json");

        private WebApiCoreSeedContext CreateContext() =>
            new WebApiCoreSeedContext(_dbContextOptions);
    }
}