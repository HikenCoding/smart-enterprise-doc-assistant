using System;
using System.Text;
using UglyToad.PdfPig;

public class DocumentService
{
    //Verwandelt die PDF-Datei in einen Text-String, damit die KI ihn verstehen kann.
    public string GetTextFromPdf(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return string.Empty;
        try
        {
            //endloser Notizzettel. Der Computer schickt sehr schnell und effizient ganz viele Textstücke hintereinander.
            var sb = new StringBuilder();
            using (var document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    sb.AppendLine(page.Text);
                }
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Fehler beim Auslesen der PDF: {ex.Message}";
        }
    }
}