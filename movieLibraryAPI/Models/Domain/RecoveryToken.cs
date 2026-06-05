namespace movieLibraryAPI.Models.Domain;

public class RecoveryToken
{
    public Guid Id { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public string TokenSalt { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? InvalidatedAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
}