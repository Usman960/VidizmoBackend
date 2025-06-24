public class UserWithRolesDto
{
    public int UserId { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }

    public List<RoleAssignments?> IndividualRoles { get; set; } = new();
    public List<RoleAssignments?> GroupRoles { get; set; } = new();
}

public class RoleAssignments
{
    public int UserOgGpRoleId { get; set; }
    public string RoleName { get; set; }
}
