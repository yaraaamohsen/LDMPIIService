using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LDMPII_DSL.ServicesInterfaces;
using LDMPII_Entities.AttachmentDtos;
using LDMPII_Entities.PdfGeneration;
using LDMPII_Helper.CustomExceptions;
using LDMPII_Helper.CustomExceptions.PdfGenerationExceptions;
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
            PdfRequiredDataValidation(token, attachmentData);

            try
            {
                var http = _httpClientFactory.CreateClient();
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var request = new PdfRequest
                {
                    Pdf = "base64-encoded PDF string",
                    Data = JsonSerializer.Deserialize<PatientData>(attachmentData.JsonOutput),
                    TemplateId = "colorectal-liquid"
                };

                var response = await http.PostAsJsonAsync(_pdfUrl, request);
                var successResponse = await HttpResponseHandler.HandleResponseAsync<PdfResponse>(response, _logger);

                if (successResponse.Status != "success" || string.IsNullOrEmpty(successResponse.ModifiedPdf))
                {
                    throw new PdfGenerationException("Invalid PDF response format");
                }

                return Convert.FromBase64String(successResponse.ModifiedPdf);
            }

            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error during PDF generation");
                throw new PdfGenerationException("Failed to process PDF response");
            }
            catch (Exception ex) when (ex is not PdfGenerationException)
            {
                _logger.LogError(ex, "Unexpected error during PDF generation.");
                throw new PdfGenerationException("An unexpected error occurred while generating the PDF.");
            }
        }

        public static void PdfRequiredDataValidation(string token, GetAttachmentDto attachmentData)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidPdfInputException("Token Not Found");

            if (attachmentData == null)
                throw new InvalidPdfInputException(nameof(attachmentData));

            if (string.IsNullOrWhiteSpace(attachmentData.JsonOutput))
                throw new InvalidPdfInputException("Attachment JSON Data Is Empty");

            if (attachmentData.SeqNum <= 0)
                throw new InvalidPdfInputException("Sequence Number Must Be Positive");
        }
    }
}
