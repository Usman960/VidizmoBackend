namespace VidizmoBackend.Models {
    public class Tag {
        public int TagId {get; set;}
        public string Title {get; set;}

        public ICollection<VideoTag> VideoTags {get; set;}
    }
}