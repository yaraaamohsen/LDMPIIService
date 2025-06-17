using System.Text.Json.Serialization;

namespace LDMPII_Entities.PdfGeneration
{
    public class PdfRequest
    {
        [JsonPropertyName("pdf")]
        public string Pdf { get; set; }
        [JsonPropertyName("data")]
        public PatientData Data { get; set; }
        [JsonPropertyName("templateId")]
        public string TemplateId { get; set; } = "Test12345";
    }
}
