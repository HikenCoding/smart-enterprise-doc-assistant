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
            return Ok("Scan gestartet");
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