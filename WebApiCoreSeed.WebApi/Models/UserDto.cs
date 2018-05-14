﻿using System.ComponentModel.DataAnnotations;

namespace WebApiCoreSeed.WebApi.Models
{
    public class UserDto
    {
        /// <summary>
        /// First name of the person.
        /// </summary>
        [Required(ErrorMessage = "You should provide a first name value")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name, family name or surname of the person.
        /// </summary>
        [Required(ErrorMessage = "You should provide a last name value")]
        [MaxLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// Email address of the person.
        /// </summary>
        [Required]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// UserName of the person.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }
    }
}
