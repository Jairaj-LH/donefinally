using System;
using System.ComponentModel.DataAnnotations;

namespace charac.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public int Likes { get; set; } = 0;

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
