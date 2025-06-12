using Microsoft.AspNetCore.Mvc;

public class PdfController : Controller
{
    private readonly PdfService _pdfService;

    public PdfController(PdfService pdfService)
    {
        _pdfService = pdfService;
    }
    [HttpGet("/pdf/generate")]
    public IActionResult GeneratePdf()
    {
        string htmlContent = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title>Test PDF</title>
    <style>
        body { font-family: Arial, sans-serif; }
        h1 { color: navy; }
        p { font-size: 14px; }
    </style>
</head>
<body>
    <h1>Hello PDF!</h1>
    <p>This is a PDF generated from HTML content.</p>
</body>
</html>
";

        var pdfBytes = _pdfService.GeneratePdf(htmlContent);

        return File(pdfBytes, "application/pdf", "test.pdf");
    }

}
