using charac.Data;
using charac.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace charac.Controllers
{
    public class CompleteinfoController : Controller
    {

        //first inject the dbcontext,bcz controller needs database access
        //to perform operations
        private readonly ApplicationDbContext _db;
        public CompleteinfoController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Details(int id)
        {
            var subject = await _db.Subjects
                          .Include(s => s.Characters)
                          .ThenInclude(c => c.briefDescription) // Access the biography related to each character
                          .Include(s => s.Acts)
                          .Where(s => s.SubId == id)  // Filter by Subject ID
                          .FirstOrDefaultAsync();


            if (subject == null)
                return NotFound();

            var viewModel = new CompleteInfoViewModel
            {
                SubId = subject.SubId,
                SubName = subject.SubName,
                SubGenre = subject.SubGenre,
                Characters = subject.Characters.Select(c => new CharacterWithBiographyViewModel
                {
                    CharId = c.CharId,
                    chName = c.chName,
                    chDescription = c.chDescription,
                    isNegative = c.isNegative,
                    briefDescription = c.briefDescription?.briefDescription
                }).ToList(),
                ActOne = subject.Acts?.FirstOrDefault()?.actOne,
                ActTwo = subject.Acts?.FirstOrDefault()?.actTwo,
                ActThree = subject.Acts?.FirstOrDefault()?.actThree

            };

        return View(viewModel); // Will look for Views/CompleteInfo/Details.cshtml

        }
    }
}
