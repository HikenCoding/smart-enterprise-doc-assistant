using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ChatService
{
    //Ermöglicht es, dass der Service Anrufe an Ollama in dem Beispiel tätigt.
    private readonly HttpClient _httpClient;
    //speichern ab, wo praktisch die KI wohnt und ihren namen
    private const string OllamaUrl = "http://localhost:11434/api/generate";
    private const string Model = "llama3";

    //Konstruktor und hier machen wir eine Dependency Injection.
    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    //Schickt PDF-Text und Frage als Paket an die KI und wir bekommen die Antwort zurück
    public async Task<string> AskQuestion(string context, string question)
    {
        // Prompt für Ollama erstellen
        string prompt = $"Beantworte die Frage nur basierend auf dem folgenden Kontext: {context}\nFrage: {question}";

        // Wir sagen, nimmm Modell 'llama3', hier meine Anweisung und schick mir die Antwort zurück.
        var requestBody = new
        {
            model = Model,
            prompt = prompt,
            stream = false
        };

        //Wir verwandeln unsere C# Schachtel in JSON-Sprache, damit Ollama sie versteht.
        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        try
        {
            /
            using var response = await _httpClient.PostAsync(OllamaUrl, content);

            //Wenn die KI die Antwort angenommen hat, dann laden wir die antwort.
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Wir suchen uns von der Ollama Antwort die 'response' aus, weil wir nur das brauchen.
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
        //Fehlerbehandlung
        catch (Exception ex)
        {
            // Fehlerhandling ggf. anpassen
            return $"Fehler beim Kommunizieren mit Ollama: {ex.Message}";
        }
    }
}