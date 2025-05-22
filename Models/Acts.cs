using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace charac.Models
{
    public class Acts
    {
        [Key, ForeignKey("Subject")]
        public int SubId { get; set; }  // This will be both the primary key and foreign key

        public string actOne { get; set; }
        public string actTwo { get; set; }
        public string actThree { get; set; }

        public virtual Subject Subject { get; set; }
    }
}
