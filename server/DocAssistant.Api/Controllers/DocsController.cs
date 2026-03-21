using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

//Alles was in dieser Datei ist, gehört zu dieser Adresse
namespace DocAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocsController : ControllerBase
    {
        //PDF Spezialst 
        private readonly DocumentService _documentService;
        //KI Spezialist
        private readonly ChatService _chatService;


        //Konstruktor wird erstellt und documentService sowie chatService werden hier aufgerufen und gespeichert
        public DocsController(DocumentService documentService, ChatService chatService)
        {
            _documentService = documentService;
            _chatService = chatService;
        }

        //Wie eine Klingel an der Haustür. Angular schickt Nachricht an 'api/docs'ask' und Methode arbeitet
        [HttpGet("scan")]
        public IActionResult Scan()
        {
            //Pdf wird ausgelesen
            var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "test-data");
            
            
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

        //Benutzer stellt eine Anfrage zu einer PDF
        [HttpPost("ask")]
public async Task<IActionResult> Ask([FromBody] ChatRequest request)
{
    if (string.IsNullOrEmpty(request.Question))
    {
        return BadRequest("Frage darf nicht leer sein, Bro");
    }

    // 1. Den Pfad zur PDF bestimmen
    var fileName = request.FileName ?? "vertrag_max_mustermann.pdf";
    var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "test-data", fileName);

    if (!System.IO.File.Exists(path))
    {
        return NotFound($"Die Datei {fileName} wurde im test-data Ordner nicht gefunden.");
    }

    // 2. Text aus der PDF extrahieren (via DocumentService)
    var pdfText = _documentService.GetTextFromPdf(path);

    // 3. Abfrage wird an Ollama gesendet (mittels ChatService)
    var response = await _chatService.AskQuestion(pdfText, request.Question);

    // 4. Antwort der KI wird verpackt und an Angular gesendet.
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