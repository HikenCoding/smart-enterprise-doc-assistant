using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocsController : ControllerBase
    {
        private readonly DocumentService _documentService;
        private readonly ChatService _chatService;

        public DocsController(DocumentService documentService, ChatService chatService)
        {
            _documentService = documentService;
            _chatService = chatService;
        }

        [HttpGet("scan")]
        public IActionResult Scan()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "test-data");
            // Hier kommt deine Scan-Logik rein...
            // 1. Pfad zum Ordner '../../test-data' definieren (relative zum Working Directory)
            var testDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "test-data");

            // 2. Ordner anlegen, falls nicht vorhanden
            if (!Directory.Exists(testDataPath))
            {
                Directory.CreateDirectory(testDataPath);
            }

            // 3. Alle .pdf-Dateien im Ordner finden
            var pdfFiles = Directory.GetFiles(testDataPath, "*.pdf");

            var resultList = new List<object>();

            // 4. Für jede Datei mit DocumentService den Text extrahieren
            foreach (var pdfPath in pdfFiles)
            {
                var content = _documentService.GetTextFromPdf(pdfPath);
                var fileName = Path.GetFileName(pdfPath);

                resultList.Add(new
                {
                    FileName = fileName,
                    Content = content
                });
            }

            // 5. Liste von Objekten mit FileName und Content zurückgeben  
            return Ok(resultList);
        }

        [HttpPost("ask")]
public async Task<IActionResult> Ask([FromBody] ChatRequest request)
{
    if (string.IsNullOrEmpty(request.Question))
    {
        return BadRequest("Frage darf nicht leer sein, Bro.");
    }

    // 1. Den Pfad zur PDF bestimmen
    var fileName = request.FileName ?? "vertrag_max_mustermann.pdf";
    var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "test-data", fileName);

    if (!System.IO.File.Exists(path))
    {
        return NotFound($"Die Datei {fileName} wurde im test-data Ordner nicht gefunden.");
    }

    // 2. Text aus der PDF extrahieren (via DocumentService)
    // Hinweis: Wir nehmen hier der Einfachheit halber die erste Datei oder die spezifische
    var pdfText = _documentService.GetTextFromPdf(path);

    // 3. Abfrage an Ollama senden (via ChatService)
    var response = await _chatService.AskQuestion(pdfText, request.Question);

    // 4. Die echte Antwort von Ollama zurückgeben
    return Ok(new { answer = response });
}
    }

    // Das Model muss AUẞERHALB der Controller-Klasse, aber innerhalb des Namespaces stehen
    public class ChatRequest
    {
        public string Question { get; set; } = string.Empty;
        public string? FileName { get; set; }
    }
}