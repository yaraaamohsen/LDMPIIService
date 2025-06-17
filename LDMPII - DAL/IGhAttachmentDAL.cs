using LDMPII_Entities;
using NT.Integration.SharedKernel.OracleManagedHelper;

namespace LDMPII_DAL
{
    public interface IGhAttachmentDAL
    {
        Task GetGhAttachmentAsync(OracleManager oracleManager, GetAttachmentDto ghAttachmentDto);
        Task SetGhAttachmentAsync(OracleManager oracleManager, SetAttachmentDto setAttachmentDto);
    }
}
