namespace YourNamespace.Models
{
    public class PartResponse
    {
        public string text { get; set; }
    }

    public class ContentResponse
    {
        public List<PartResponse> parts { get; set; }
    }

    public class Candidate
    {
        public ContentResponse content { get; set; }
    }

    public class GeminiResponse
    {
        public List<Candidate> candidates { get; set; }
    }
}
