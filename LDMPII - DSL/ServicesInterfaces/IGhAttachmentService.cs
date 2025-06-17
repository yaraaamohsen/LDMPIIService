using LDMPII_Entities.AttachmentDtos;

namespace LDMPII_DSL.ServicesInterfaces
{
    public interface IGhAttachmentService
    {
        Task GetGhAttachmentAsync(GetAttachmentDto GetAttachmentDto);
        Task SetGhAttachmentAsync(SetAttachmentDto setAttachmentDto);
    }
}
