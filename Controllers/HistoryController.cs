using charac.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace charac.Controllers
{
    public class HistoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HistoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> MyHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var history = await _db.UserActivityHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.Timestamp)
                .ToListAsync();

            return View(history);
        }
    }
}
