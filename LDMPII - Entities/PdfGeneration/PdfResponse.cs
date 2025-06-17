using System.Text.Json.Serialization;

namespace LDMPII_Entities.PdfGeneration
{
    public class PdfResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("modifiedPdf")]
        public string ModifiedPdf { get; set; }
    }
}
