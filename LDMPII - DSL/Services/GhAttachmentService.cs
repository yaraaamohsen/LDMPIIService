using LDMPII_DAL;
using LDMPII_Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NT.Integration.SharedKernel.OracleManagedHelper;

namespace LDMPII_DSL.Services
{
    public class GhAttachmentService(IGhAttachmentDAL _ghAttachmentDAL, IConfiguration _configuration, ILogger<GhAttachmentService> _logger)
    {
        public async Task GetGhAttachmentAsync(GhAttachmentDto ghAttachmentDto)
        {
            try
            {
                using (var oracleManager = new OracleManager(_configuration.GetConnectionString("ConnectionString")))
                {
                    await oracleManager.OpenConnectionAsync();
                    _logger.LogInformation("Connection Opened");
                    await _ghAttachmentDAL.GetGhAttachmentAsync(oracleManager, ghAttachmentDto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure");

            }

        }
    }
}
