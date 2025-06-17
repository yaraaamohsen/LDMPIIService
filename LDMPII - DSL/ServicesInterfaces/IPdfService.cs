using LDMPII_Entities.AttachmentDtos;

namespace LDMPII_DSL.ServicesInterfaces
{
    public interface IPdfService
    {
        Task<byte[]> GeneratePdfAsync(string token, GetAttachmentDto attachmentData);
    }
}
