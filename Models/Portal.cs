namespace VidizmoBackend.Models
{
    public class Portal
    {
        public int PortalId { get; set; } // Unique identifier for the portal
        public string Name { get; set; } // Name of the portal
        public DateTime CreatedAt { get; set; } // Timestamp when the portal was created

        // Foreign key to Organization
        public int OrganizationId { get; set; }
        
        public ICollection<UserPortalRole> UserPortalRoles { get; set; } // Collection of user portal roles associated with this portal
        public ICollection<Video> Videos { get; set; } // Collection of videos associated with this portal
        public ICollection<Group> Groups { get; set; } // Collection of groups associated with this portal
    }
}