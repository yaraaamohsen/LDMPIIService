using System.Data;
using LDMPII_Entities.AttachmentDtos;
using Microsoft.Extensions.Configuration;
using NT.Integration.SharedKernel.OracleManagedHelper;
using Oracle.ManagedDataAccess.Client;

namespace LDMPII_DAL
{
    public class GhAttachmentDAL(IConfiguration _configuration) : IGhAttachmentDAL
    {
        public async Task GetGhAttachmentAsync(OracleManager oracleManager, GetAttachmentDto getAttachmentDto)
        {
            oracleManager.CommandParameters.Add("P_JSON", OracleDbType.Clob, null, ParameterDirection.Output);
            oracleManager.CommandParameters.Add("P_Seq_num", OracleDbType.Int32, null, ParameterDirection.Output);

            await oracleManager.ExcuteNonQueryAsync($"{_configuration.GetSection("StoredProcedures:GetAttachmentApi").Value}", CommandType.StoredProcedure);

            getAttachmentDto.JsonOutput = oracleManager.CommandParameters["P_JSON"].Value?.ToString() ?? string.Empty;
            getAttachmentDto.SeqNum = (Int32)oracleManager.CommandParameters["P_Seq_num"].Value;
        }

        public async Task SetGhAttachmentAsync(OracleManager oracleManager, SetAttachmentDto setAttachmentDto)
        {
            oracleManager.CommandParameters.Add("P_file", OracleDbType.Blob, setAttachmentDto.FileContent, ParameterDirection.Input);
            oracleManager.CommandParameters.Add("P_seq_num", OracleDbType.Int32, setAttachmentDto.SeqNum, ParameterDirection.Input);
            oracleManager.CommandParameters.Add("P_status", OracleDbType.Int32, setAttachmentDto.Status, ParameterDirection.Input);

            await oracleManager.ExcuteNonQueryAsync($"{_configuration.GetSection("StoredProcedures:SetAttachmentApi").Value}", CommandType.StoredProcedure);
        }
    }
}

