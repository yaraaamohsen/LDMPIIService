using LDMPII_DAL;
using LDMPII_DSL.ServicesInterfaces;
using LDMPII_Entities.AttachmentDtos;
using LDMPII_Helper.CustomExceptions.DatabaseExceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NT.Integration.SharedKernel.OracleManagedHelper;

namespace LDMPII_DSL.Services
{
    public class GhAttachmentService(IGhAttachmentDAL _ghAttachmentDAL,
        IConfiguration _configuration,
        ILogger<GhAttachmentService> _logger) : IGhAttachmentService
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
                _logger.LogError(ex, "Database Error While Getting Attachment");
                throw new DatabaseOperationException("Database Operation Failed", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error Getting Attachment");
                throw new DatabaseOperationException("Unexpected Attachment Error", ex);
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
            catch (OracleCustomException ex)
            {
                _logger.LogError(ex, "Database Error While Getting Attachment");
                throw new DatabaseOperationException("Database Operation Failed", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error Getting Attachment");
                throw new DatabaseOperationException("Unexpected Error", ex);
            }
        }
    }
}
