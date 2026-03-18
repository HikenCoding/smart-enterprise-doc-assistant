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
            // Hier kommt deine Ask-Logik rein...
            return Ok("KI antwortet bald");
        }
    }

    // Das Model muss AUẞERHALB der Controller-Klasse, aber innerhalb des Namespaces stehen
    public class ChatRequest
    {
        public string Question { get; set; } = string.Empty;
        public string? FileName { get; set; }
    }
}