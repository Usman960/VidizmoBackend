namespace VidizmoBackend.Models
{
    public class ScopedToken
    {
        public int ScopedTokenId { get; set; }

        // The organization context this token applies to (optional if global)
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }

        // Hashed token for validation/storage (NEVER store raw token)
        public string TokenHash { get; set; }

        // List of granted permissions like ["video:upload", "video:download"]
        public string ScopeJson { get; set; }  // Stored as serialized JSON array

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        public ICollection<Video> Videos { get; set; } = new List<Video>();
    }
}