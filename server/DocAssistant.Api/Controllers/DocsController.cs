using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class DocsController : ControllerBase
{
    private readonly DocumentService _documentService;

    public DocsController(DocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpGet("Scan")]
    public ActionResult<List<string>> Scan()
    {
        // Suche alle PDF-Dateien im angegebenen Ordner
        var pdfDirectory = Path.Combine("..", "..", "test-data");
        if (!Directory.Exists(pdfDirectory))
        {
            return NotFound($"Ordner '{pdfDirectory}' wurde nicht gefunden.");
        }

        var pdfFiles = Directory.GetFiles(pdfDirectory, "*.pdf");
        var results = new List<string>();

        foreach (var file in pdfFiles)
        {
            var text = _documentService.GetTextFromPdf(file);
            results.Add(text);
        }

        return Ok(results);
    }
}