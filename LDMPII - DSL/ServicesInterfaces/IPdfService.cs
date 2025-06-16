using LDMPII_Entities;

namespace LDMPII_DSL.ServicesInterfaces
{
    public interface IPdfService
    {
        Task<byte[]> GeneratePdfAsync(string token, GhAttachmentDto attachmentData);
    }
}
