using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LDMPII_DSL.ServicesInterfaces;
using LDMPII_Entities;
using LDMPII_Entities.PdfGeneration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LDMPII_DSL.Services
{
    public class PdfService : IPdfService
    {
        private readonly ILogger<PdfService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly string _pdfUrl;


        public PdfService(ILogger<PdfService> logger, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _pdfUrl = _config.GetSection("PdfSetting:PdfUrl").Value
            ?? throw new ArgumentNullException("PdfSetting:PdfUrl Configuration Is Missing");
        }
        public async Task<byte[]> GeneratePdfAsync(string token, GetAttachmentDto attachmentData)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty", nameof(token));

            if (attachmentData == null)
                throw new ArgumentNullException(nameof(attachmentData));
            try
            {
                var http = _httpClientFactory.CreateClient();
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new PdfRequest
                {
                    Pdf = "base64-encoded PDF string",
                    Data = JsonSerializer.Deserialize<PatientData>(attachmentData.JsonOutput),
                    TemplateId = "colorectal-liquid"
                };

                var response = await http.PostAsJsonAsync(_pdfUrl, request);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<PdfResponse>();
                return Convert.FromBase64String(result.ModifiedPdf);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while generating PDF");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error");
                throw;
            }
        }
    }
}
