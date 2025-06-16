using System.Data;
using LDMPII_Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace LDMPII_DAL
{
    public class GhAttachmentDAL(IConfiguration configuration, ILogger<GhAttachmentDAL> logger) : IGhAttachmentDAL
    {
        public async Task<GhAttachmentDto> GetGhAttachmentAsync()
        {
            using var connection = new OracleConnection(configuration.GetSection("ConnectionString").Value);
            await connection.OpenAsync();

            using var cmd = new OracleCommand("G42_LDM.get_GH_ATTACHMENT_API", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            OracleParameter jsonParam = new OracleParameter("P_JSON", OracleDbType.Clob, ParameterDirection.Output);

            OracleParameter seqNumParam = new OracleParameter("P_Seq_num", OracleDbType.Int64, ParameterDirection.Output);

            cmd.Parameters.AddRange(new[] { jsonParam, seqNumParam });

            try
            {
                await cmd.ExecuteNonQueryAsync();
                logger.LogInformation("Query Excuted");
                var jsonOutput = ((OracleClob)jsonParam.Value).Value.ToString(); // HERE: We Declared That jsonParam Is Clob, It Be Clob After Excuting, And We Move Line By Line, So When It Arrived Here Its Already Clob?

                int seqNum = seqNumParam.Value != DBNull.Value ? Convert.ToInt32(seqNumParam.Value) : 0;

                return new GhAttachmentDto
                {
                    JsonOutput = jsonOutput,
                    SeqNum = seqNum
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing stored procedure");
                throw;
            }
        }

        public async Task SetAttachmentAsync(byte[] file, int seqNum, int status)
        {
            using var connection = new OracleConnection(configuration.GetSection("ConnectionString").Value);
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

