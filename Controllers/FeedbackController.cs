using Microsoft.AspNetCore.Mvc;
using charac.Models;
using System.Linq;
using System;
using charac.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace charac.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FeedbackController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var messages = _db.Feedbacks.OrderByDescending(f => f.Timestamp).ToList();
            return View(messages);
        }

        [HttpPost]
        public IActionResult Submit([FromBody] Feedback model)
        {
            model.Timestamp = DateTime.Now; // optional: set timestamp here
            _db.Feedbacks.Add(model);
            _db.SaveChanges();

            return Ok(new
            {
                id = model.Id,
                username = model.Username,
                message = model.Message,
                timestamp = model.Timestamp,
                likes = model.Likes
            });
        }

        [HttpPost]
        public async Task<IActionResult> Like([FromBody] LikeRequest request)
        {
            Console.WriteLine("Like request received for ID: " + request.Id);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                Console.WriteLine("Unauthorized: No user ID found.");
                return Unauthorized();
            }

            var like = await _db.FeedbackLikes
                .FirstOrDefaultAsync(l => l.FeedbackId == request.Id && l.UserId == userId);

            if (like == null)
            {
                like = new FeedbackLike
                {
                    FeedbackId = request.Id,
                    UserId = userId,
                    IsLiked = true
                };
                _db.FeedbackLikes.Add(like);
            }
            else
            {
                like.IsLiked = !like.IsLiked;
            }

            await _db.SaveChangesAsync();

            // ✅ Count total likes again
            var totalLikes = await _db.FeedbackLikes
                .CountAsync(l => l.FeedbackId == request.Id && l.IsLiked);

            // ✅ Update the Feedback table's Likes column
            var feedback = await _db.Feedbacks.FindAsync(request.Id);
            if (feedback != null)
            {
                feedback.Likes = totalLikes;
                await _db.SaveChangesAsync();
            }

            Console.WriteLine("Likes updated. Total likes now: " + totalLikes);

            return Ok(new { likes = totalLikes, userLiked = like.IsLiked });
        }





        // Helper class for deserializing the like request
        public class LikeRequest
        {
            public int Id { get; set; }
        }


    }
}
