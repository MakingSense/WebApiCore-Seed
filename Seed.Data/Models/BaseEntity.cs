using System;
using System.ComponentModel.DataAnnotations;

namespace Seed.Data.Models
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
