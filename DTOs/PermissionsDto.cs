namespace VidizmoBackend.DTOs
{
    public class PermissionsDto
    {
        public List<PermissionDto> Permissions { get; set; } = new();
    }

    public class PermissionDto
    {
        public string Action { get; set; }
        public string Entity { get; set; }
    }
}
