namespace VidizmoBackend.DTOs
{
    public class GroupRolesDto
    {
        public int UserId { get; set; }
        public List<RoleAssignments> GroupRoles { get; set; } = new();
    }
}