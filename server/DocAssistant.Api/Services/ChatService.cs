using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ChatService
{
    private readonly HttpClient _httpClient;
    private const string OllamaUrl = "http://localhost:11434/api/generate";
    private const string Model = "llama3"; // Passe ggf. den Modellnamen an

    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> AskQuestion(string context, string question)
    {
        // Prompt für Ollama erstellen
        string prompt = $"Beantworte die Frage nur basierend auf dem folgenden Kontext: {context}\nFrage: {question}";

        // Request Body zusammenbauen
        var requestBody = new
        {
            model = Model,
            prompt = prompt,
            stream = false
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        try
        {
            using var response = await _httpClient.PostAsync(OllamaUrl, content);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Ollama gibt in "response" das Antwort-Feld zurück
            using var jsonDoc = JsonDocument.Parse(responseString);
            if (jsonDoc.RootElement.TryGetProperty("response", out var responseElement))
            {
                return response?.Response ?? "Entschuldigung, ich konnte keine Antwort generieren.";
            }
            else
            {
                return "Keine Antwort von Ollama erhalten.";
            }
        }
        catch (Exception ex)
        {
            // Fehlerhandling ggf. anpassen
            return $"Fehler beim Kommunizieren mit Ollama: {ex.Message}";
        }
    }
}