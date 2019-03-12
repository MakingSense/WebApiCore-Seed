namespace Seed.Data.EF
{
    using Seed.Data.Models;
    using System;
    using System.Linq;

    /// <summary>
    /// Helper class to seed sample data into a <see cref="WebApiCoreSeedContext"/>
    /// </summary>
    public static class DatabaseSeed
    {
        /// <summary>
        /// Initializes a <see cref="WebApiCoreSeedContext"/> with sample Data
        /// </summary>
        /// <param name="userContext">Context to be initialized with sample data</param>
        public static void Initialize(UserContext userContext)
        {
            userContext.Database.EnsureCreated();

            if (!userContext.Users.Any())
            { 
                var users = new[]
                {
                    new User { CreatedOn = DateTime.Now, Email = "noreply@makingsense.com", FirstName = "John", LastName = "Doe", UserName = "JohnDoe" },
                };

                userContext.Users.AddRange(users);
            }

            userContext.SaveChanges();
        }
    }
}
