namespace VidizmoBackend.Models
{
    public class Role
    {
        public int RoleId { get; set; } // Unique identifier for the role
        public string Name { get; set; } // Name of the role (e.g., "Admin", "User")
        public string Description { get; set; } // Description of the role's purpose or permissions
        public DateTime CreatedAt { get; set; } // Timestamp when the role was created
        public int CreatedByUserId { get; set; } // Foreign key to the User who created this role
        public User CreatedByUser { get; set; } // Navigation to the User who created this role      

        // Navigation properties
        public ICollection<UserOrgRole> UserOrgRoles { get; set; } // Collection of user roles associated with this role
        public ICollection<RolePermission> RolePermissions { get; set; } // Collection of role permissions associated with this role
    }
}