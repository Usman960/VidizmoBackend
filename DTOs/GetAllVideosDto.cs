namespace VidizmoBackend.DTOs 
{
    public class GetAllVideosDto
    {
        public int VideoId { get; set; } // Unique identifier for the video
        public string Title { get; set; } // Title of the video
        public string Description { get; set; } // Description of the video
    }
}