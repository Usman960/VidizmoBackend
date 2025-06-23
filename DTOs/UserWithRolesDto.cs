public class UserWithRolesDto
{
    public int UserId { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public List<RoleAssignments?> Roles { get; set; }
}

public class RoleAssignments
{
    public int UserOgGpRoleId { get; set; }
    public string RoleName { get; set; }
}
