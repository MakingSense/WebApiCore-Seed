using Seed.Data.Models;
using System;

namespace Seed.Api.Models
{
    public class UserDto
    {
        public UserDto() { }

        public UserDto(User user)
        {
            Id = user.Id;
            CreatedAt = user.CreatedOn;
            UpdatedAt = user.UpdatedOn;
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserName = user.UserName;
            Email = user.Email;
        }

        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
