namespace VidizmoBackend.DTOs
{
    public class NotifyUploadDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string BlobName { get; set; }
        public long FileSize { get; set; }
        public int OrgId { get; set; }
        public string OriginalFileName { get; set; }
    }
}