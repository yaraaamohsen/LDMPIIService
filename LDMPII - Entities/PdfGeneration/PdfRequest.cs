namespace LDMPII_Entities.PdfGeneration
{
    public class PdfRequest
    {
        public string Pdf { get; set; }
        public PatientData Data { get; set; }
        public string TemplateId { get; set; } = "Test12345";
    }
}
