using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace charac.Models
{
    public class Subject
    {
        [Key] // Marking CharId explicitly as the primary key

        public int SubId {get; set;}
        public string SubName { get; set; }
        public string SubGenre { get; set; }

        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<Acts> Acts { get; set; }

    }
}
