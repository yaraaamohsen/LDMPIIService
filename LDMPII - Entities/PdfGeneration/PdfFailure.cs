using System.Text.Json.Serialization;

namespace LDMPII_Entities.PdfGeneration
{
    public class PdfFailure
    {
        [JsonPropertyName("errorStatusCode")]
        public int ErrorStatusCode { get; set; }
        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
