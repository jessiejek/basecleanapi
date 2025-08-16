namespace BaseStarterPack.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }

    // matches your DB column (NOT NULL)
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // matches your DB column (BIT NOT NULL)
    public bool Revoked { get; set; } = false;

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public bool IsActive => !Revoked && DateTime.UtcNow < ExpiresAtUtc;
}
