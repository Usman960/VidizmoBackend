public class TokenViewDto
{
    public int TokenId { get; set; }
    public string ScopeJson { get; set; } // Serialized permissions
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
}
