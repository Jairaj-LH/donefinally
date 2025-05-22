using charac.Data;
using charac.Migrations;
using charac.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace charac.Controllers
{
    public class ActController : Controller
    {

        //first inject dbcontext (DI),bcz controller needs to access db
        private readonly ApplicationDbContext _db;
        public ActController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var acts = await _db.Acts.Include(a => a.Subject).ToListAsync();
            return View(acts);
        }
        //Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var acts = await _db.Acts.Include(a => a.Subject)
                .FirstOrDefaultAsync(m => m.SubId == id);
            if (acts == null) return NotFound();
            return View(acts);

        }
        //create-get
        public async Task<IActionResult> Create()
        {
            ViewData["SubjectId"] = new SelectList(_db.Subjects, "SubId", "SubName");
            return View();

        }
        //create-post
        [HttpPost]
        public async Task<IActionResult> Create(Acts act)
        {
            _db.Acts.Add(act);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        //edit-get
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var act = await _db.Acts.FindAsync(id);

            if (act == null) return NotFound();

            ViewData["SubjectId"] = new SelectList(_db.Subjects, "SubId", "SubName", act.SubId);


            return View(act);

        }
        //edit-post
        [HttpPost]

        public async Task<IActionResult> Edit(int id, Acts act)
        {
            if (id != act.SubId) return NotFound();

            _db.Acts.Update(act);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        //Delete-Get
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var act = await _db.Acts.Include(a => a.Subject)
                .FirstOrDefaultAsync(m => m.SubId == id);

            if (act == null) return NotFound();

            return View(act);
        }
        //delete-post
        [HttpPost]

        public async Task<IActionResult> Delete(int id)
        {
            var act = await _db.Acts.FindAsync(id);
            _db.Remove(act);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

      


    }
}
