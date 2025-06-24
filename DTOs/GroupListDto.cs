namespace VidizmoBackend.DTOs
{
    public class GroupListDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public List<GroupUserDto> Users { get; set; }
    public List<GroupRoleDto> Roles { get; set; }
}

public class GroupUserDto
{
    public int UserId { get; set; }
    public string Email { get; set; }
}

public class GroupRoleDto
{
    public int UserOgGpRoleId { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
}

}