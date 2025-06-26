public class TokenViewDto
{
    public int TokenId { get; set; }
    public string Permissions { get; set; } // Serialized permissions
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
}
