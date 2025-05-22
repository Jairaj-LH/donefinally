using charac.Data;
using charac.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace charac.Controllers
{
    public class CharacterController : Controller
    {

        //first inject dbcontext,bcz controller needs access to the database
        private readonly ApplicationDbContext _db;

        public CharacterController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Character/
        public async Task<IActionResult> Index()
        {
            var characters = await _db.Characters
                .Include(c => c.Subject)
                .Include(c => c.briefDescription)
                .ToListAsync();

            return View(characters);
        }
        // GET: /Character/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var character = await _db.Characters
                .Include(c => c.Subject)
                .Include(c => c.briefDescription)
                .FirstOrDefaultAsync(m => m.CharId == id);

            if (character == null)
            {
                return NotFound();
            }
             return View(character);
    }
        //get
        public async Task<IActionResult> Create()
        {
            ViewData["Subjects"] = new SelectList(_db.Subjects, "SubId", "SubName");
            return View();

        }
        //HTTPpost
        [HttpPost]
        public async Task<IActionResult> Create(Character character)
        {
            ViewData["Subjects"] = new SelectList(_db.Subjects, "SubId", "Name"); // "SubId" must match property in model

            _db.Characters.Add(character);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");

           
        }
        //edit -get
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var character= await _db.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }
            ViewData["Subjects"]= new SelectList(_db.Subjects, "SubId", "SubName", character.SubId);
            return View(character);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,Character character)
        {
            if (id != character.CharId) return NotFound();

            _db.Characters.Update(character);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        //delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var character = await _db.Characters.Include(c => c.Subject)
                .FirstOrDefaultAsync(m =>m.CharId == id);

            if (character == null) return NotFound();

            return View(character);
        }
        //Delete-post
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if(id==null) return NotFound();

            var character = await _db.Characters.FindAsync(id);
            _db.Characters.Remove(character);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> BySubject(int id)
        {
            var characters = await _db.Characters
                .Include(c => c.Subject)
                .Where(c => c.SubId == id)
                .ToListAsync();

            var subject = await _db.Subjects.FindAsync(id);
            ViewBag.SubjectName = subject?.SubName;
            ViewBag.SubjectId = id;


            return View("BySubject", characters);
        }
    }
}
