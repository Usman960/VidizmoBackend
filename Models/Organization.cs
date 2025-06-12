namespace VidizmoBackend.Models
{
    public class Organization
    {
        public int OrganizationId { get; set; } // Unique identifier for the organization
        public string Name { get; set; } // Name of the organization
        public string Description { get; set; } // Description of the organization
        public DateTime CreatedAt { get; set; } // Timestamp when the organization was created

        public int CreatedByUserId { get; set; } // FK to User
        public User CreatedByUser { get; set; } // Navigation to User

        public ICollection<Portal> Portals { get; set; } // Navigation to Portals
    }
}
