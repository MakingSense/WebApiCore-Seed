using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Seed.Api.IntegrationTests.Controllers.TestData;
using Seed.Api.IntegrationTests.Generics;
using Seed.Api.Models;
using Seed.Data.EF;
using Seed.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Seed.Api.IntegrationTests.Controllers
{
    public class UserControllerTests : IClassFixture<ApiTestFixture>
    {
        private const string ResouceUri = "api/users/";

        private readonly HttpClient _httpClient;
        private readonly DbContextOptions<WebApiCoreSeedContext> _dbContextOptions;

        public UserControllerTests(ApiTestFixture fixture)
        {
            _httpClient = fixture.Client;
            _dbContextOptions = fixture.DbContextOptions;
        }

        [Fact]
        public async Task GetAll_ShouldReturnUsers()
        {
            // Arrange
            var sampleUsers = new List<User>()
            {
                GetSampleUser(),
                GetSampleUser(),
                GetSampleUser()
            };

            using (var context = CreateContext())
            {
                await context.Users.AddRangeAsync(sampleUsers);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _httpClient.GetAsync(ResouceUri);

            // Assert
            Assert.True(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var users = JsonConvert.DeserializeObject<List<UserDto>>(await result.Content.ReadAsStringAsync());
            foreach (var expected in sampleUsers)
            {
                var actual = users.SingleOrDefault(u => u.Id == expected.Id);
                Assert.Equal(expected.FirstName, actual.FirstName);
                Assert.Equal(expected.LastName, actual.LastName);
                Assert.Equal(expected.UserName, actual.UserName);
                Assert.Equal(expected.Email, actual.Email);
            }
        }

        [Fact]
        public async Task Get_ShouldReturnAUser_WhenIdExists()
        {
            Guid guid;
            using (var dbcontext = CreateContext())
            {
                var userdb = (await dbcontext.Users.FirstOrDefaultAsync());

                if (userdb == null)
                {
                    userdb = (await dbcontext.Users.AddAsync(new User
                    {
                        Email = "anemail@email.com",
                        CreatedBy = "creator",
                        CreatedOn = DateTime.Now,
                        FirstName = "firstName",
                        Id = Guid.NewGuid(),
                        LastName = "lastName",
                        UserName = "userName"
                    })).Entity;
                    await dbcontext.SaveChangesAsync();
                }

                guid = userdb.Id;
            }
            var request = ResouceUri + guid.ToString();

            var result = await _httpClient.GetAsync(request);

            result.EnsureSuccessStatusCode();

            var user = JsonConvert.DeserializeObject<User>(await result.Content.ReadAsStringAsync());

            Assert.Equal(guid, user.Id);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenIdNotExists()
        {
            var guid = Guid.NewGuid();

            var request = ResouceUri + guid.ToString();

            var result = await _httpClient.GetAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        #region Post Tests

        [Fact]
        public async Task Post_ShouldCreateAUser_WhenAUserIsGiven()
        {
            var user = new InputUserDto()
            {
                Email = "testpersonIntegration@email.com",
                FirstName = "testName",
                LastName = "testLastName",
                UserName = "testpersonIntegration"
            };

            var result = await _httpClient.PostAsync(ResouceUri, CreateContent(user));

            result.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
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

            var result = await _httpClient.PostAsync(ResouceUri, CreateContent(user));

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

                if (dbUser == null)
                {
                    dbUser = (await dbcontext.Users.AddAsync(new User
                    {
                        Email = "anemail@email.com",
                        CreatedBy = "creator",
                        CreatedOn = DateTime.Now,
                        FirstName = "firstName",
                        Id = Guid.NewGuid(),
                        LastName = "lastName",
                        UserName = "userName"
                    })).Entity;

                    await dbcontext.SaveChangesAsync();
                }
            }

            var user = new InputUserDto()
            {
                Email = "testpersonIntegration@email.com",
                FirstName = "testName",
                LastName = "testLastName",
                UserName = "testpersonIntegration"
            };

            var requestUri = $"{ResouceUri}{dbUser.Id.ToString()}";

            var result = await _httpClient.PutAsync(requestUri, CreateContent(user));
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

            var user = new InputUserDto()
            {
                Email = "testpersonIntegration@email.com",
                FirstName = "testName",
                LastName = "testLastName",
                UserName = "testpersonIntegration"
            };

            var requestUri = $"{ResouceUri}{id.ToString()}";

            var result = await _httpClient.PutAsync(requestUri, CreateContent(user));

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
                if (dbUser == null)
                {
                    dbUser = (await dbcontext.Users.AddAsync(new User
                    {
                        Email = "anemail@email.com",
                        CreatedBy = "creator",
                        CreatedOn = DateTime.Now,
                        FirstName = "firstName",
                        Id = Guid.NewGuid(),
                        LastName = "lastName",
                        UserName = "userName"
                    })).Entity;

                    await dbcontext.SaveChangesAsync();
                }
            }

            var requestUri = $"{ResouceUri}{dbUser.Id.ToString()}";

            var result = await _httpClient.PutAsync(requestUri, CreateContent(user));

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

            var result = await _httpClient.DeleteAsync($"{ResouceUri}{userId.ToString()}");

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

            var result = await _httpClient.DeleteAsync($"{ResouceUri}{userId.ToString()}");

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

        private User GetSampleUser(Guid? id = null)
        {
            return new User
            {
                Id = id ?? Guid.NewGuid(),
                Email = "johndoe@gmail.com",
                FirstName = "John",
                LastName = "Doe",
                UserName = "fn",
                CreatedOn = DateTime.Now
            };
        }
    }
}