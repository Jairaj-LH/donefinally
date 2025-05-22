namespace charac.ViewModels
{
    public class CharacterWithBiographyViewModel
    {
        public int CharId { get; set; }
        public string chName { get; set; }
        public string chDescription { get; set; }
        public bool isNegative { get; set; }
        public string briefDescription { get; set; }
    }

    public class CompleteInfoViewModel
    {
        public int SubId { get; set; }
        public string SubName { get; set; }
        public string SubGenre { get; set; }

        public List<CharacterWithBiographyViewModel> Characters { get; set; }
        public string ActOne { get; set; }
        public string ActTwo { get; set; }
        public string ActThree { get; set; }
    }
}
