namespace VidizmoBackend.Models {
    public class VideoTag {
        public int VideoTagId {get; set;}
        public int VideoId {get; set;}
        public Video Video {get; set;} // Navigation to the Video this tag belongs to
        public int TagId {get; set;}
        public Tag Tag {get; set;} // Navigation to the Tag this video is tagged with
    }
}