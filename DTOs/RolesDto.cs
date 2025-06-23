namespace VidizmoBackend.DTOs
{
    public class RolesDto
    {
        public List<RoleResDto> rolesDto { get; set; }
    }

    public class RoleResDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; } // Name of the role
        public string Description { get; set; } // Description of the role
        public PermissionsDto Permissions { get; set; } = new(); // Permissions associated with the role
    }
}