using System.Data;
using LDMPII_Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NT.Integration.SharedKernel.OracleManagedHelper;
using Oracle.ManagedDataAccess.Client;

namespace LDMPII_DAL
{
    public class GhAttachmentDAL(IConfiguration _configuration, ILogger<GhAttachmentDAL> logger) : IGhAttachmentDAL
    {
        public async Task GetGhAttachmentAsync(OracleManager oracleManager, GhAttachmentDto ghAttachmentDto)
        {
            oracleManager.CommandParameters.Add("P_JSON", OracleDbType.Clob, null, ParameterDirection.Output);
            oracleManager.CommandParameters.Add("P_Seq_num", OracleDbType.Int32, null, ParameterDirection.Output);

            await oracleManager.ExcuteNonQueryAsync($"{_configuration.GetSection("StoredProcedures:GetAttachmentApi").Value}", CommandType.StoredProcedure);

            ghAttachmentDto.JsonOutput = oracleManager.CommandParameters["P_JSON"].Value.ToString(); // Here Need To Edit
            ghAttachmentDto.SeqNum = (Int32)oracleManager.CommandParameters["P_JSON"].Value;
        }

        public async Task SetAttachmentAsync(byte[] file, int seqNum, int status)
        {
            using var connection = new OracleConnection(_configuration.GetSection("ConnectionString").Value);
            await connection.OpenAsync();

            using var cmd = new OracleCommand("G42_LDM.set_GH_ATTACHMENT_API", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            var blobParams = new OracleParameter("P_file", OracleDbType.Blob)
            {
                Value = file
            };

            cmd.Parameters.AddRange(new OracleParameter[]
            {
                blobParams,
                new OracleParameter("P_seq_num", seqNum),
                new OracleParameter("P_status", status)
            });

            try
            {
                await cmd.ExecuteNonQueryAsync();
                logger.LogInformation("Set Attachment Excuted");
            }
            catch
            {
                logger.LogError("Error executing is Set Attachment");
            }
        }
    }
}

