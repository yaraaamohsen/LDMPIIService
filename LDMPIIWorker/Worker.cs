using LDMPII_DSL.ServicesInterfaces;
using LDMPII_Entities.AttachmentDtos;


namespace LDMPIIWorker
{
    public class Worker : BackgroundService
    {
        private readonly int _normalIntervalInMinutes;
        private readonly int _errorRetryIntervalInMinutes;
        private readonly ILogger<Worker> _logger;
        private readonly IAuthService _authService;
        private readonly IPdfService _pdfService;
        private readonly IGhAttachmentService _attachmentService;
        private readonly IConfiguration _config;

        public Worker(ILogger<Worker> logger,
        IAuthService authService,
        IPdfService pdfService,
        IGhAttachmentService attachmentService,
        IConfiguration config)
        {
            _logger = logger;
            _authService = authService;
            _pdfService = pdfService;
            _attachmentService = attachmentService;
            _config = config;

            _normalIntervalInMinutes = IntervalValidation(_config, "Intervals:NormalIntervalInMinutes");
            _errorRetryIntervalInMinutes = IntervalValidation(_config, "Intervals:ErrorRetryIntervalInMinutes");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timer has Started Successfully at : {}", DateTimeOffset.Now);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Started");
                    // 1. Get Token
                    var token = await _authService.GetTokenAsync();
                    _logger.LogInformation("Successfully GEnerate Token: {Token}...", token);

                    // 2. Generate Attachment
                    await ProcessAttachment(token, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Graceful shutdown
                    _logger.LogInformation("Worker stopping due to cancellation request");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during worker execution. Retrying in {RetryInterval}", _errorRetryIntervalInMinutes);
                    await Task.Delay(_errorRetryIntervalInMinutes, stoppingToken);
                }

                await Task.Delay(_normalIntervalInMinutes, stoppingToken);
            }
        }

        private async Task ProcessAttachment(string? token, CancellationToken stoppingToken)
        {
            try
            {
                // 1. Get Atachment
                var attachmentDto = new GetAttachmentDto();

                await _attachmentService.GetGhAttachmentAsync(attachmentDto);
                if (string.IsNullOrEmpty(attachmentDto.JsonOutput))
                {
                    _logger.LogWarning("No attachment data received");
                    return;
                }
                _logger.LogInformation("Processing attachment with SeqNum: {SeqNum}", attachmentDto.SeqNum);

                // 2. GeneratePdf
                var pdfContent = await _pdfService.GeneratePdfAsync(token, attachmentDto);

                // 3. Set Attachment
                var setAttachmentDto = new SetAttachmentDto
                {
                    FileContent = pdfContent,
                    SeqNum = attachmentDto.SeqNum,
                    Status = 1
                };

                await _attachmentService.SetGhAttachmentAsync(setAttachmentDto);
                _logger.LogInformation("Successfully processed attachment {SeqNum}", attachmentDto.SeqNum);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed To Process Attachment");
                throw;
            }
        }

        private static int IntervalValidation(IConfiguration config, string configPath)
        {
            var value = config.GetValue<int?>(configPath);
            if (value is null or <= 0)
            {
                throw new ArgumentException($"Invalid interval value for {configPath}. Must be a positive integer.");
            }
            return value.Value;
        }
    }
}

