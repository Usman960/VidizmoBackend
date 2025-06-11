namespace VidizmoBackend.Models
{
    public class Video
    {
        public int VideoId { get; set; } // Unique identifier for the video
        public string Filename { get; set; } // Name of the video file
        public string Title { get; set; } // Title of the video
        public string Description { get; set; } // Description of the video
        public double FileSize { get; set; } // Size of the video file in bytes
        public string FilePath { get; set; } // Path to the video file in the storage system
        public string FileFormat { get; set; } // Format of the video file (e.g., mp4, avi, etc.)
        public DateTime UploadedAt  { get; set; } // Timestamp when the video was uploaded
        public int UploadedByUserId { get; set; } // Foreign key to the User who uploaded this video
        // Navigation properties
        public ICollection<VideoTag> VideoTags {get; set;}  
    }
}