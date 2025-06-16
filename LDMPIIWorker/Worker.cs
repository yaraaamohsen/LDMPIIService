using LDMPII_DAL;
using LDMPII_DSL.ServicesInterfaces;

namespace LDMPIIWorker
{
    public class Worker(ILogger<Worker> _logger,
        IAuthService _authService,
        IPdfService _pdfService,
        IGhAttachmentDAL _ghAttachmentDAL) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timer has Started Successfully at : {}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                var attachment = await _ghAttachmentDAL.GetGhAttachmentAsync();
                try
                {
                    _logger.LogInformation("Started");
                    // 1. Get Token
                    var token = await _authService.GetTokenAsync();
                    _logger.LogInformation("Successfully GEnerate Token: {Token}...", token);

                    // 2. Get Attachment
                    _logger.LogInformation("Generate Attachment With SeqNum: {SeqNum}", attachment.SeqNum);

                    var pdfContent = await _pdfService.GeneratePdfAsync(token, attachment);

                    // 3. Set Attachment
                    await _ghAttachmentDAL.SetAttachmentAsync(pdfContent, attachment.SeqNum, 1);

                    _logger.LogInformation("Successfully set attachment with SeqNum: {SeqNum}", attachment.SeqNum);
                }
                catch (Exception ex) when (ex is not TaskCanceledException)
                {
                    _logger.LogError(ex, "Token acquisition failed");
                    if (_ghAttachmentDAL != null && attachment?.SeqNum != null)
                    {
                        await _ghAttachmentDAL.SetAttachmentAsync(null, attachment.SeqNum, 2);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}

