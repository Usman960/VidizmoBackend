namespace VidizmoBackend.Models {
    public class User {
        public int UserId { get; set; }
        public string Firstname { get; set; }
        public string? Lastname { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Group> GroupsCreated { get; set; }

        // Separate navigation for users who were added TO groups
        public ICollection<UserGroup> UserGroupsAddedTo { get; set; }

        // Separate navigation for users who performed the addition
        public ICollection<UserGroup> UserGroupsAddedByMe { get; set; }

        public ICollection<Video> Videos { get; set; }
        public ICollection<UserPortalRole> UserPortalRoles { get; set; }
        public ICollection<Organization> Organizations { get; set; }
        public ICollection<ScopedToken> TokensCreated { get; set; }
        public ICollection<ScopedToken> ScopedTokensReceived { get; set; }

    }
}