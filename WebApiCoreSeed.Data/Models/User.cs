namespace WebApiCoreSeed.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a person in our system
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// First name of the person.
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name, family name or surname of the person.
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Email address of the person.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// UserName of the person.
        /// </summary>
        [Required]
        public string UserName { get; set; }
    }
}
