using charac.Data;
using charac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace charac.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SubjectController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Allow all authenticated users to see their subjects
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subjects = await _db.Subjects
                                    .Where(s => s.UserId == userId)
                                    .ToListAsync();
            return View(subjects);
        }

        // Allow all authenticated users to create subjects
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Subject subject)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            subject.UserId = userId;

            _db.Add(subject);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Details can be viewed by the owner user only (no role needed)
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.SubId == id && s.UserId == userId);
            if (subject == null)
                return NotFound();
            return View(subject);
        }

        // Allow all authenticated users to edit their subjects
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.SubId == id && s.UserId == userId);
            if (subject == null)
                return NotFound();
            return View(subject);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Subject subject)
        {
            if (id != subject.SubId)
                return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (subject.UserId != userId)
                return Unauthorized();

            _db.Subjects.Update(subject);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Allow all authenticated users to delete their subjects
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.SubId == id && s.UserId == userId);
            if (subject == null)
                return NotFound();
            return View(subject);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeletePost(int SubId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.SubId == SubId && s.UserId == userId);
            if (subject == null)
                return NotFound();

            _db.Subjects.Remove(subject);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
