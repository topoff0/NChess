namespace Account.Core.Entities;

public class EmailVerificationCode
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiryAt { get; set; }

    public bool IsExpired => CreatedAt >= ExpiryAt;
    public bool IsActive => !IsExpired;
}
