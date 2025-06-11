namespace VidizmoBackend.Models
{
    public class Permission
    {
        public int PermissionId { get; set; } // Unique identifier for the permission
        public string Action { get; set; } // Action that the permission allows (e.g., "Create", "Read", "Update", "Delete")
        public string Entity { get; set; } 

        // Navigation properties
        public ICollection<RolePermission> RolePermissions { get; set; } // Collection of role permissions associated with this permission
    }
}