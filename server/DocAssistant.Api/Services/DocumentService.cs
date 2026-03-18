using System;
using System.Text;
using UglyToad.PdfPig;

public class DocumentService
{
    public string GetTextFromPdf(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return string.Empty;
        try
        {
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
            // Log error or handle it appropriately
            return $"Fehler beim Auslesen der PDF: {ex.Message}";
        }
    }
}