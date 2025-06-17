using System.Text.Json.Serialization;

namespace LDMPII_Entities.PdfGeneration
{
    public class PatientData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("dob")]
        public string Dob { get; set; }
        [JsonPropertyName("mrn")]
        public string Mrn { get; set; }
    }
}