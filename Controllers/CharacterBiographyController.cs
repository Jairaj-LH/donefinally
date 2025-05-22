using charac.Data;
using charac.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace charac.Controllers
{
    public class CharacterBiographyController : Controller
    {
        //first inject dbcontext ,bcz controller needs access to db
        private readonly ApplicationDbContext _db;

        public CharacterBiographyController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var bios = _db.CharactersBiography.Include(m => m.Character);
            return View(await bios.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var bio = await _db.CharactersBiography
                .Include(b => b.Character)
                .FirstOrDefaultAsync(m => m.charId == id);

            if (bio == null) return NotFound();

            return View(bio);
        }
        //create-get
        public async Task<IActionResult> Create()
        {
            ViewData["charId"] = new SelectList(_db.Characters, "CharId", "chName");
            return View();
        }
        //create-post
        [HttpPost]

        public async Task<IActionResult> Create(CharacterBiography bio)
        {
            _db.Add(bio);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bio = await _db.CharactersBiography.FindAsync(id);
            if (bio == null) return NotFound();

            ViewData["charId"] = new SelectList(_db.Characters, "CharId", "chName", bio.charId);

            return View(bio);
        }
        //edit-post
        [HttpPost]

        public async Task<IActionResult> Edit(int? id, CharacterBiography bio)
        {
            if (id == null) return NotFound();

            _db.CharactersBiography.Update(bio);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");

        }
        //delete get
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bio = await _db.CharactersBiography
                  .Include(b => b.Character)
                .FirstOrDefaultAsync(m => m.charId == id); 
            
            if (bio == null) return NotFound();

            return View(bio);
        }

        //deletepost
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null) return NotFound();

            var bio = await _db.CharactersBiography.FindAsync(id);

            _db.Remove(bio);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
                
        public async Task<IActionResult> ByCharacter(int? id)
        {
            if (id == null) return NotFound();

            var charactersbiography = await _db.CharactersBiography
                .Include(c => c.Character)
                .Where(c => c.charId == id)
                .ToListAsync();
            var character = await _db.Characters.FindAsync(id);
            ViewBag.CharacterName = character?.chName;
            ViewBag.CharacterId = id;
            ViewBag.SubjectId = character.SubId; // ✅ Add this line


            return View("ByCharacter", charactersbiography);

        }
        private bool CharacterBiographyExists(int id)
        {
            return _db.CharactersBiography.Any(e => e.charId == id);
        }

    }
}
