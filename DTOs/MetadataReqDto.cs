namespace VidizmoBackend.DTOs
{
    public class MetadataReqDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<string>? Tags { get; set; }
    }
}