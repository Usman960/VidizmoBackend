namespace VidizmoBackend.DTOs
{
    public class IndividualRolesDto
    {
        public int UserId { get; set; }
        public List<RoleAssignments?> IndividualRoles { get; set; } = new();
    }

    public class RoleAssignments
    {
        public int UserOgGpRoleId { get; set; }
        public string RoleName { get; set; }
    }
}