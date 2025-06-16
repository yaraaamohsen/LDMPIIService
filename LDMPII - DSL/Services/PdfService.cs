using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LDMPII_DSL.ServicesInterfaces;
using LDMPII_Entities;
using LDMPII_Entities.PdfGeneration;
using Microsoft.Extensions.Logging;

namespace LDMPII_DSL.Services
{
    public class PdfService(ILogger<PdfService> _logger, IHttpClientFactory _httpClientFactory) : IPdfService
    {
        public async Task<byte[]> GeneratePdfAsync(string token, GhAttachmentDto attachmentData)
        {
            var http = _httpClientFactory.CreateClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new PdfRequest
            {
                Pdf = "base64-encoded PDF string",
                Data = JsonSerializer.Deserialize<PatientData>(attachmentData.JsonOutput),
                TemplateId = "olorectal-liquid"
            };

            var response = await http.PostAsJsonAsync("https://dev-pdf-generator.g4?healthcare.ai/api/pdf/update", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PdfResponse>();
            return Convert.FromBase64String(result.ModifiedPdf);
        }
    }
}
