namespace VidizmoBackend.Models
{
    public class Group
    {
        public int GroupId { get; set; } // Unique identifier for the group
        public string Name { get; set; } // Name of the group
        public DateTime CreatedAt { get; set; } // Timestamp when the group was created
        public int OrganizationId { get; set; } // Foreign key to the Organization this group belongs to
        public Organization Organization { get; set; } // Navigation to the Organization this group belongs to
        public int CreatedByUserId { get; set; } // Foreign key to the User who created this group
        public User CreatedByUser { get; set; } // Navigation to the User who created this group
        // Navigation properties
        public ICollection<UserGroup> UserGroups { get; set; } // Collection of user groups associated with this group
        public ICollection<UserOgGpRole> UserOgGpRoles { get; set; } // Collection of user roles associated with this group
    }
}