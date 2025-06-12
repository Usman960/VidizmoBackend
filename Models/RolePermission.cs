namespace VidizmoBackend.Models
{
    public class RolePermission
    {
        public  int RolePermissionId { get; set; } // Unique identifier for the role permission association
        public int RoleId { get; set; } // Foreign key to the Role
        public Role Role { get; set; } // Navigation to the Role
        public int PermissionId { get; set; } // Foreign key to the Permission
        public Permission Permission { get; set; } // Navigation to the Permission
    }
}