namespace charac.Models
{
    public class FeedbackLike
    {
        public int Id { get; set; }
        public int FeedbackId { get; set; }
        public string UserId { get; set; }  // Identity user ID

        public bool IsLiked { get; set; } = true;
    }
}