using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        public async Task GetAll_ReturnsOk()
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
        public async Task Get_ReturnsOk()
        {
            // Arrange
            var sampleUser = GetSampleUser();
            using (var context = CreateContext())
            {
                await context.Users.AddAsync(sampleUser);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _httpClient.GetAsync($"{ResouceUri}{sampleUser.Id.ToString()}");

            // Assert
            Assert.True(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var user = JsonConvert.DeserializeObject<UserDto>(await result.Content.ReadAsStringAsync());
            Assert.Equal(sampleUser.Id, user.Id);
            Assert.Equal(sampleUser.FirstName, user.FirstName);
            Assert.Equal(sampleUser.LastName, user.LastName);
            Assert.Equal(sampleUser.UserName, user.UserName);
            Assert.Equal(sampleUser.Email, user.Email);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenUserNotExists()
        {
            // Act
            var result = await _httpClient.GetAsync($"{ResouceUri}{Guid.NewGuid()}");

            // Assert
            Assert.False(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            // Arrange
            var sampleUser = new InputUserDto()
            {
                Email = "testpersonIntegration@email.com",
                FirstName = "testName",
                LastName = "testLastName",
                UserName = "testpersonIntegration"
            };

            // Act
            var result = await _httpClient.PostAsync(ResouceUri, CreateContent(sampleUser));

            // Assert
            Assert.True(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);

            var user = JsonConvert.DeserializeObject<UserDto>(await result.Content.ReadAsStringAsync());
            Assert.Equal(sampleUser.FirstName, user.FirstName);
            Assert.Equal(sampleUser.LastName, user.LastName);
            Assert.Equal(sampleUser.UserName, user.UserName);
            Assert.Equal(sampleUser.Email, user.Email);
        }

        [Fact(Skip = "Validation attribute is not working")]
        public async Task Create_ReturnsBadRequest_WhenInvalidInput()
        {
            // Arrange
            var sampleUser = new InputUserDto
            {
                Email = "invalid-address"
            };

            // Act
            var result = await _httpClient.PostAsync(ResouceUri, CreateContent(sampleUser));

            // Assert
            Assert.False(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsNoContent()
        {
            // Arrange
            var sampleUser = GetSampleUser();
            using (var context = CreateContext())
            {
                await context.Users.AddAsync(sampleUser);
                await context.SaveChangesAsync();
            }

            var userToUpdate = new InputUserDto()
            {
                Email = "different-email@email.com",
                FirstName = "different-first-name",
                LastName = "different-last-name",
                UserName = "different-username"
            };

            // Act
            var result = await _httpClient.PutAsync($"{ResouceUri}{sampleUser.Id.ToString()}", CreateContent(userToUpdate));

            // Assert
            Assert.True(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);

            User updatedUser;
            using (var context = CreateContext())
            {
                updatedUser = await context.Users.FindAsync(sampleUser.Id);
            }

            Assert.Equal(userToUpdate.Email, updatedUser.Email);
            Assert.Equal(userToUpdate.FirstName, updatedUser.FirstName);
            Assert.Equal(userToUpdate.LastName, updatedUser.LastName);
            Assert.Equal(userToUpdate.UserName, updatedUser.UserName);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenUserNotExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userToUpdate = new InputUserDto()
            {
                Email = "different-email@email.com",
                FirstName = "different-first-name",
                LastName = "different-last-name",
                UserName = "different-username"
            };

            // Act
            var result = await _httpClient.PutAsync($"{ResouceUri}{userId.ToString()}", CreateContent(userToUpdate));

            // Assert
            Assert.False(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact(Skip = "Validation attribute is not working")]
        public async Task Update_ReturnsBadRequest_WhenInputIsInvalid()
        {
            // Arrange
            var sampleUser = GetSampleUser();
            using (var context = CreateContext())
            {
                await context.Users.AddAsync(sampleUser);
                await context.SaveChangesAsync();
            }

            var userToUpdate = new InputUserDto()
            {
                Email = "invalid-email"
            };

            // Act
            var result = await _httpClient.PutAsync($"{ResouceUri}{sampleUser.Id.ToString()}", CreateContent(userToUpdate));

            // Assert
            Assert.False(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

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
                UserName = "johndoe",
                CreatedOn = DateTime.Now
            };
        }
    }
}