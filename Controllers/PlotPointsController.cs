using charac.Data;
using charac.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace charac.Controllers
{
    public class PlotPointsController : Controller
    {
        //first inject dbcontext dependency injection bcz controller needs dB access
        private readonly ApplicationDbContext _db;
        public PlotPointsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var plotpoints = await _db.Plotpoints.ToListAsync();
            return View(plotpoints);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var plotpoints = await _db.Plotpoints.FindAsync(id);
            if (plotpoints == null)
            {
                return NotFound();
            }
            return View (plotpoints);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Subjects"] = new SelectList(await _db.Subjects.ToListAsync(), "SubId", "SubName");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Plotpoints plotpoints)
        {
            _db.Add(plotpoints);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var plotpoints = await _db.Plotpoints.FindAsync(id);
            if(plotpoints == null)
                return NotFound();

            return View(plotpoints);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Plotpoints plotpoints)
        {
            if (id == null) return NotFound();

            _db.Update(plotpoints);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var plotpoints = await _db.Plotpoints.FindAsync(id);
            if (plotpoints == null)
                return NotFound();
            return View(plotpoints);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null) return NotFound();

            var plotpoits = await _db.Plotpoints.FindAsync(id);
            if (plotpoits != null)
            {
                _db.Remove(plotpoits);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
