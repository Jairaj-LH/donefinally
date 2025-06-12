using charac.Data;
using charac.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;
using YourNamespace.Models;

namespace charac.Controllers
{
    public class CharacterController : Controller
    {

        //first inject dbcontext,bcz controller needs access to the database
        private readonly ApplicationDbContext _db;
        private readonly PdfService _pdfService;


        public CharacterController(ApplicationDbContext db, PdfService pdfService)
        {
            _db = db;
            _pdfService=pdfService;
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
        [HttpGet("/characters/pdf")]
        public async Task<IActionResult> GeneratePdf()
        {
            var characters = await _db.Characters
                .Include(c => c.Subject)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("<html><head><meta charset='utf-8' /><title>Characters PDF</title>");
            sb.AppendLine("<style>table { border-collapse: collapse; width: 100%; }");
            sb.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; }");
            sb.AppendLine("th { background-color: #f2f2f2; }</style></head><body>");
            sb.AppendLine("<h1>Characters List</h1>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>ID</th><th>Name</th><th>Description</th><th>Negative?</th><th>Subject</th></tr>");

            foreach (var c in characters)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{c.CharId}</td>");
                sb.AppendLine($"<td>{c.chName}</td>");
                sb.AppendLine($"<td>{c.chDescription}</td>");
                sb.AppendLine($"<td>{(c.isNegative ? "Yes" : "No")}</td>");
                sb.AppendLine($"<td>{c.Subject?.SubName ?? "N/A"}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table></body></html>");

            var html = sb.ToString();
            var pdfBytes = _pdfService.GeneratePdf(html);

            return File(pdfBytes, "application/pdf", "CharactersList.pdf");
        }

    }
}
