using VidizmoBackend.DTOs;
public class TokenDto
{
    public PermissionsDto Permissions { get; set; } = new PermissionsDto(); // Permissions associated with the token
    // Duration in hours (e.g., 2 means expires in 2 hours from now)
    public int DurationInHours { get; set; } 
}
