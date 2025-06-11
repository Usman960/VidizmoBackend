namespace VidizmoBackend.Models {
    public class User {
        public int UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<UserGroup> UserGroups { get; set; }
        public ICollection<Video> Videos { get; set; }
        public ICollection<UserPortalRole> UserPortalRoles { get; set; }
    }

}