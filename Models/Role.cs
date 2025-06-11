namespace VidizmoBackend.Models
{
    public class Role
    {
        public int RoleId { get; set; } // Unique identifier for the role
        public string Name { get; set; } // Name of the role (e.g., "Admin", "User")
        public string Description { get; set; } // Description of the role's purpose or permissions
        public DateTime CreatedAt { get; set; } // Timestamp when the role was created
        public int CreatedByUserId { get; set; } // Foreign key to the User who created this role
        
        // Navigation properties
        public ICollection<UserPortalRole> UserPortalRoles { get; set; } // Collection of user-portal role associations
        public ICollection<RolePermission> RolePermissions { get; set; } // Collection of role permissions associated with this role
    }
}