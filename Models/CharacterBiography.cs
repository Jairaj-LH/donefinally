using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace charac.Models
{
    public class CharacterBiography
    {
        [Key, ForeignKey("Character")]
        public int charId { get; set; }
        public string briefDescription { get; set; }

        public virtual Character Character { get; set; }


    }
}
