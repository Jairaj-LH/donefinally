using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace charac.Models
{
    public class Plotpoints
    {
        [Key, ForeignKey("Subject")]
        public int SubId { get; set; }

        public string PlotPoint1 { get; set; }
        public string PlotPoint2 { get; set; }
        public Subject Subject { get; set; }

    }
}
