using LDMPII_DAL;
using LDMPII_DSL.ServicesInterfaces;
using LDMPII_Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NT.Integration.SharedKernel.OracleManagedHelper;

namespace LDMPII_DSL.Services
{
    public class GhAttachmentService(IGhAttachmentDAL _ghAttachmentDAL, IConfiguration _configuration, ILogger<GhAttachmentService> _logger) : IGhAttachmentService
    {
        public async Task GetGhAttachmentAsync(GetAttachmentDto GetAttachmentDto)
        {
            try
            {
                using (var oracleManager = new OracleManager(_configuration.GetConnectionString("ConnectionString")))
                {
                    await oracleManager.OpenConnectionAsync();
                    _logger.LogInformation("Connection Opened Successfull To Get Attachment");
                    await _ghAttachmentDAL.GetGhAttachmentAsync(oracleManager, GetAttachmentDto);
                }
            }
            catch (OracleCustomException ex)
            {
                _logger.LogError(ex, "Database Error Occurred While Getting Attachment");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error Getting Attachment");
                throw;
            }
        }

        public async Task SetGhAttachmentAsync(SetAttachmentDto setAttachmentDto)
        {
            try
            {
                using (var oracleManager = new OracleManager(_configuration.GetConnectionString("ConnectionString")))
                {
                    await oracleManager.OpenConnectionAsync();
                    _logger.LogInformation("Connection Opened To Set Attachment");
                    await _ghAttachmentDAL.SetGhAttachmentAsync(oracleManager, setAttachmentDto);
                }
            }
            catch
            {
                _logger.LogError("Error executing is Set Attachment");

            }
        }
    }
}
