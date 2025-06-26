using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace charac.Models
{
    public class Subject
    {
        [Key]
        public int SubId { get; set; }

        public string SubName { get; set; }

        public string SubGenre { get; set; }

        // NEW: Foreign key to the Identity user
        public string UserId { get; set; }

        // OPTIONAL: Navigation property
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<Acts> Acts { get; set; }
    }
}
