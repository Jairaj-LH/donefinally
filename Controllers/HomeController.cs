using System.Diagnostics;
using charac.Models;
using Microsoft.AspNetCore.Mvc;

namespace charac.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;



        public HomeController(ILogger<HomeController> logger, IEmailService emailService,IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
            _emailService = emailService; 
        }
        public async Task<IActionResult> SendTestEmail()
        {
            await _emailService.SendEmailAsync("jairajlh08@gmail.com", "Test Email", "<p>This is a test email.</p>");
            return Content("Email Sent");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
                return Content("No file selected");

            var permittedExtensions = new[] { ".pdf", ".docx", ".jpg", ".png" };
            var ext = Path.GetExtension(uploadedFile.FileName).ToLowerInvariant();

            if (!permittedExtensions.Contains(ext))
            {
                return Content("Invalid file type.");
            }

            if (uploadedFile.Length > 5 * 1024 * 1024) // 5MB limit
            {
                return Content("File too large.");
            }

            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(stream);
            }

            ViewBag.FileName = fileName;
            return View("UploadSuccess");
        }

        public IActionResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            var path = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            if (!System.IO.File.Exists(path))
                return NotFound();

            var mimeType = "application/octet-stream";
            return File(System.IO.File.ReadAllBytes(path), mimeType, fileName);
        }
    }
}
