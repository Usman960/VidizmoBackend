namespace VidizmoBackend.Models {
    public class UserPortalRole {
        public int UserPortalRoleId {get; set;}
        public int UserId {get; set;}
        public int PortalId {get; set;}
        public int RoleId {get; set;}
        public DateTime AssignedAt {get; set;}
    }
}