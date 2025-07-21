using charac.Data;
using charac.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace charac.Controllers
{
    public class SWFormatController : Controller
    {

        //first inject the dbcontext dependency injection
        //bcz controoler needs database access to manipulate data

        private readonly ApplicationDbContext _db;

        public SWFormatController(ApplicationDbContext db)
        {
            _db= db;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.SWFormat.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var swFormat = await _db.SWFormat.FirstOrDefaultAsync(m => m.scno == id);

            if (swFormat == null)
            {
                return NotFound();
            }
            return View(swFormat);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SWFormat swFormat)
        {
            _db.SWFormat.Add(swFormat);
            await _db.SaveChangesAsync();
            return View(swFormat);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var swFormat = await _db.SWFormat.FindAsync(id);

            if (swFormat == null)
            {
                return NotFound();
            }
            return View(swFormat);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, SWFormat swFormat)
        {
            if (id == null) 
                return NotFound();

            _db.SWFormat.Update(swFormat);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var swFormat = await _db.SWFormat.FirstOrDefaultAsync(m => m.scno == id);

            if (swFormat == null) return NotFound();

            return View(swFormat);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if(id==null) return NotFound();

            var swFormat= await _db.SWFormat.FirstOrDefaultAsync(m=> m.scno == id);
            _db.Remove(swFormat);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GroupedByLocation()
        {
            var groupedScenes = _db.SWFormat
                .GroupBy(s => s.Location)
                .Select(group => new SceneGroupViewModel
                {
                    Location = group.Key,
                    Scenes = group.Select(s => new SceneViewModel
                    {
                        scno = s.scno,
                        INTOREXT = s.INTOREXT,
                        Time = s.Time,
                        charname = s.charname,
                        dialogue = s.dialogue,
                        props = s.props,
                        equips = s.equips,
                        costumes = s.costumes,
                        artistinvolved = s.artistinvolved
                    }).ToList()
                }).ToList();

            return View(groupedScenes); // Looks for Views/Scene/GroupedByLocation.cshtml
        }
    }
}
