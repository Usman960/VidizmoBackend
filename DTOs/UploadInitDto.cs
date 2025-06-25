namespace VidizmoBackend.DTOs
{
    public class UploadInitDto
    {
        public string OriginalFileName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OrgId { get; set; }
    }

}