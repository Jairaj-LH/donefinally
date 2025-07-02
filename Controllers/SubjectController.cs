using charac.Data;
using charac.Models;
using charac.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace charac.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<SubjectController> _logger;
        private readonly IUserActivityLogger _activityLogger;


        public SubjectController(ApplicationDbContext db, ILogger<SubjectController> logger, IUserActivityLogger activityLogger)
        {
            _db = db;
            _logger = logger;
            _activityLogger = activityLogger;


        }

        // Allow all authenticated users to see their subjects
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("User {UserId} is viewing their subjects.", userId);
            MetricsRegistry.MyCustomCounter.Inc();
            var subjects = await _db.Subjects
                                    .Where(s => s.UserId == userId)
                                    .ToListAsync();
            return View(subjects);
        }

        // Allow all authenticated users to create subjects
        [Authorize]
        public IActionResult Create()
        {
            _logger.LogInformation("User {UserId} accessed the Create Subject page.", User.FindFirstValue(ClaimTypes.NameIdentifier));

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Subject subject)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            subject.UserId = userId;
            subject.CreatedAt = DateTime.UtcNow;

            _db.Add(subject);
            await _db.SaveChangesAsync();
            _logger.LogInformation("User {UserId} created a new subject: {SubjectName}", userId, subject.SubName);
            await _activityLogger.LogAsync(userId, "Create Subject", $"Created subject: {subject.SubName}");


            return RedirectToAction("Index");
        }

        // Details can be viewed by the owner user only (no role needed)
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.SubId == id && s.UserId == userId);
            if (subject == null)
            {
                _logger.LogWarning("User {UserId} tried to access details of non-existing or unauthorized subject ID {SubjectId}.", userId, id);

                return NotFound();
            }
               
            return View(subject);
        }

        // Allow all authenticated users to edit their subjects
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.SubId == id && s.UserId == userId);
            if (subject == null)
            {
                _logger.LogWarning("User {UserId} tried to edit non-existing or unauthorized subject ID {SubjectId}.", userId, id);
                return NotFound();


            }
            _logger.LogInformation("User {UserId} accessed edit page for subject ID {SubjectId}.", userId, id);

            return View(subject);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Subject subject)
        {
            if (id != subject.SubId)
            {
                _logger.LogWarning("Edit failed: Route ID {RouteId} does not match subject ID {ModelId}.", id, subject.SubId);
                return NotFound();

            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (subject.UserId != userId)
            {
                _logger.LogWarning("User {UserId} tried to edit subject ID {SubjectId} they don't own.", userId, id);
                return Unauthorized();

            }

            _db.Subjects.Update(subject);
            await _db.SaveChangesAsync();
            _logger.LogInformation("User {UserId} edited subject ID {SubjectId}.", userId, id);

            return RedirectToAction("Index");
        }

        //for metrics
        [Authorize]
        public async Task<IActionResult> Antagonists()
        {
            var stopwatch = Stopwatch.StartNew(); // For Prometheus timing

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var negativeCharacters = await _db.Subjects
                .Where(s => s.UserId == userId)
                .Include(s => s.Characters)
                .SelectMany(s => s.Characters)
                .Where(c => c.isNegative)
                .Select(c => new Character
                {
                    CharId = c.CharId,
                    chName = "Negative " + c.chName,
                    isNegative = c.isNegative,
                    SubId = c.SubId
                })
                .ToListAsync();

            stopwatch.Stop();
            MetricsRegistry.CharacterProcessingDuration
                .WithLabels("Antagonists", "all") // or pass a specific subjectId if you’re filtering by one
                .Observe(stopwatch.Elapsed.TotalSeconds);

            return View(negativeCharacters); // Or return Json(negativeCharacters)
        }


        // Allow all authenticated users to delete their subjects
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.SubId == id && s.UserId == userId);
            if (subject == null)
            {
                _logger.LogWarning("User {UserId} tried to delete non-existing or unauthorized subject ID {SubjectId}.", userId, id);
                return NotFound();

            }
            _logger.LogInformation("User {UserId} accessed delete confirmation for subject ID {SubjectId}.", userId, id);

            return View(subject);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeletePost(int SubId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.SubId == SubId && s.UserId == userId);
            if (subject == null)
            {
                _logger.LogWarning("User {UserId} attempted to delete a subject that does not exist or they do not own. Subject ID: {SubjectId}", userId, SubId);
                return NotFound();

            }

            _db.Subjects.Remove(subject);
            await _db.SaveChangesAsync();
            _logger.LogInformation("User {UserId} deleted subject ID {SubjectId}.", userId, SubId);

            return RedirectToAction("Index");
        }
    }
}
