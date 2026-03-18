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

public class ChatRequest
{
    public string Question { get; set; }
    public string FileName { get; set; } // Optional
}

private readonly ChatService _chatService;

public DocsController(DocumentService documentService, ChatService chatService)
{
    _documentService = documentService;
    _chatService = chatService;
}

// POST: api/Docs/Ask
[HttpPost("Ask")]
public async Task<ActionResult<string>> Ask([FromBody] ChatRequest request)
{
    if (request == null || string.IsNullOrWhiteSpace(request.Question))
    {
        return BadRequest("Eine Frage muss gestellt werden.");
    }

    // Standardverzeichnis
    var pdfDirectory = Path.Combine("..", "..", "test-data");
    if (!Directory.Exists(pdfDirectory))
    {
        return NotFound($"Ordner '{pdfDirectory}' wurde nicht gefunden.");
    }

    string filePath;
    // Wenn FileName angegeben, diese verwenden, sonst das erste PDF nehmen
    if (!string.IsNullOrWhiteSpace(request.FileName))
    {
        filePath = Path.Combine(pdfDirectory, request.FileName);
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound($"Datei '{request.FileName}' wurde nicht gefunden.");
        }
    }
    else
    {
        var pdfFiles = Directory.GetFiles(pdfDirectory, "*.pdf");
        if (pdfFiles.Length == 0)
        {
            return NotFound("Keine PDF-Dateien im Verzeichnis gefunden.");
        }
        filePath = pdfFiles.First();
    }

    var context = _documentService.GetTextFromPdf(filePath);

    var answer = await _chatService.AskQuestion(context, request.Question);

    return Ok(answer);
}