namespace YourNamespace.Models
{
    public class Part
    {
        public string text { get; set; }
    }

    public class Content
    {
        public List<Part> parts { get; set; }
    }

    public class GeminiRequest
    {
        public List<Content> contents { get; set; }
    }
}
