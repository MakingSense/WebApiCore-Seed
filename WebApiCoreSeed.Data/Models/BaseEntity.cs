using System;
using System.ComponentModel.DataAnnotations;

namespace WebApiCoreSeed.Data.Models
{
    public class BaseEntity
    {
        /// <summary>
        /// Unique property that identifies a certain user from all others.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

    }
}
