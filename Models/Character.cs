using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace charac.Models
{
    public class Character
    {
        [Key] // Marking CharId explicitly as the primary key

        public int CharId { get; set; }
        public string chName { get; set; }
        public string chDescription { get; set; }
        public bool isNegative {  get; set; }
        public virtual CharacterBiography briefDescription { get; set; }

        // Foreign key to Subject
        [ForeignKey("Subject")]
        public int SubId { get; set; }

        // Navigation property to Subject
        public virtual Subject Subject { get; set; }

    }
}
