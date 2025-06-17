namespace LDMPII_Entities.AttachmentDtos
{
    public class SetAttachmentDto
    {
        public int SeqNum { get; set; }
        public byte[] FileContent { get; set; }
        public int Status { get; set; }
    }
}
