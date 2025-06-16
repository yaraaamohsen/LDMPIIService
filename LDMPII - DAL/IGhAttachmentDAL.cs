using LDMPII_Entities;
using NT.Integration.SharedKernel.OracleManagedHelper;

namespace LDMPII_DAL
{
    public interface IGhAttachmentDAL
    {
        Task GetGhAttachmentAsync(OracleManager oracleManager, GhAttachmentDto ghAttachmentDto);
        Task SetAttachmentAsync(byte[] fileContent, int seqNum, int status);
    }
}
