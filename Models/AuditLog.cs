using VidizmoBackend.Models;

public class AuditLog
{
    public int AuditLogId { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public int? TokenId { get; set; }
    public ScopedToken ScopedToken { get; set; }
    public string RequestUrl { get; set; } 
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }
    public DateTime Timestamp { get; set; }
}
