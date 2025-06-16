using LDMPII_Entities;

namespace LDMPII_DAL
{
    public interface IGhAttachmentDAL
    {
        Task<GhAttachmentDto> GetGhAttachmentAsync();
        Task SetAttachmentAsync(byte[] fileContent, int seqNum, int status);
    }
}
