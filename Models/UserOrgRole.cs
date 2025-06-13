namespace VidizmoBackend.Models {
    public class UserOrgRole {
        public int UserOrgRoleId {get; set;}
        public int UserId {get; set;}
        public User User {get; set;} // Navigation to the User this role belongs to
        public int OrganizationId {get; set;}
        public Organization  Organization {get; set;} // Navigation to the Organization this role belongs to
        public int RoleId {get; set;}
        public Role Role {get; set;} // Navigation to the Role this user has in the portal
        public DateTime AssignedAt {get; set;}
        public int AssignedByUserId {get; set;}
        public User AssignedByUser {get; set;}
        public string Status {get; set;} // Status of the user portal role (e.g., "Active", "Revoked")
        public DateTime? RevokedAt {get; set;} // Timestamp when the role was revoked, if applicable
        public int? RevokedByUserId {get; set;} // Foreign key to the User who revoked this role, if applicable
        public User? RevokedByUser {get; set;} // Navigation to the User who revoked this role, if applicable
    }
}