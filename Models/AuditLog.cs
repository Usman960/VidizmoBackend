namespace VidizmoBackend.Models
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }

        public int? PerformedById { get; set; }
        public User? PerformedBy { get; set; }

        public int? TokenId { get; set; }
        public ScopedToken? Token { get; set; }

        public string Action { get; set; } = null!;
        public string Entity { get; set; } = null!;
        public DateTime Timestamp { get; set; }

        public string Payload { get; set; } = null!;  // Contains route + body params as JSON
    }

}