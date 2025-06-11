namespace VidizmoBackend.Models
{
    public class Organization
    {
        public int OrganizationId { get; set; } // Unique identifier for the organization
        public string Name { get; set; } // Name of the organization
        public string Description { get; set; } // Description of the organization
        public DateTime CreatedAt { get; set; } // Timestamp when the organization was created
        public int CreaetdByUserId { get; set; } // Foreign key to the User who created this organization
        
        // Navigation properties
        public ICollection<Portal> Portals { get; set; } // Collection of portals associated with this organization
    }
}