namespace VidizmoBackend.DTOs 
{
    public class MetadataResDto
    {
        public int VideoId { get; set; } // Unique identifier for the video
        public string Title { get; set; } // Title of the video
        public string Description { get; set; } // Description of the video
        public List<string> Tags { get; set; } // List of tags associated with the video
        public DateTime UploadDate { get; set; } // Date when the video was uploaded
        public string UploadedBy { get; set; } // Username of the user who uploaded the video
        public double FileSize { get; set; } // Size of the video file in bytes
        public string Format { get; set; }
    }
}