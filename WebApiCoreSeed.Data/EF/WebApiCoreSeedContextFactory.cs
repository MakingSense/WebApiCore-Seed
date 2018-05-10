namespace WebApiCoreSeed.Data.EF
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.IO;

    /// <summary>
    /// Factory for creating <see cref="WebApiCoreSeedContext"/> instances using the connections string from appsettings.
    /// </summary>
    public class WebApiCoreSeedContextFactory : IDesignTimeDbContextFactory<WebApiCoreSeedContext>
    {
        private const string ConnectionString = "DefaultConnection";
        private const string StartupProjectName = "WebApiCoreSeed.WebApi";

        /// <summary>
        /// Initialize a <see cref="WebApiCoreSeedContext"/> with default configurations.
        /// </summary>
        /// <returns>
        /// new <see cref="WebApiCoreSeedContext"/> with path pointing to the base directory and current environment.
        /// </returns>
        public WebApiCoreSeedContext Create()
        {
            var environmentName = Environment.GetEnvironmentVariable("Hosting:Environment");
            var basePath = AppContext.BaseDirectory;

            return Create(basePath, environmentName);
        }

        /// <inheritdoc/>
        public WebApiCoreSeedContext CreateDbContext(string[] args)
        {
            return Create(
                Directory.GetCurrentDirectory().Replace($"WebApiCoreSeed.WebApi", string.Empty),
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
        }

        private WebApiCoreSeedContext Create(string basePath, string environmentName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath($"{basePath}{StartupProjectName}")
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();
            var connectionString = config.GetConnectionString(ConnectionString);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException($"Could not find a connection string named {ConnectionString}. Path is {basePath}{StartupProjectName}");
            }

            return Create(connectionString);
        }

        private WebApiCoreSeedContext Create(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebApiCoreSeedContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return new WebApiCoreSeedContext(optionsBuilder.Options);
        }
    }
}