using System.Collections.Generic;

namespace charac.Models
{
    public class SceneViewModel
    {
        public int scno { get; set; }
        public string INTOREXT { get; set; }
        public string Time { get; set; }
        public string charname { get; set; }
        public string dialogue { get; set; }
        public string props { get; set; }
        public string equips { get; set; }
        public string costumes { get; set; }
        public string artistinvolved { get; set; }
    }

    public class SceneGroupViewModel
    {
        public string Location { get; set; }
        public List<SceneViewModel> Scenes { get; set; }
    }
}
