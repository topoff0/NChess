namespace Account.Core.Entities;

public sealed class EmailVerificationCode
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string HashedCode { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiryAt { get; private set; }

    public bool IsUsed { get; private set; }
    public bool IsManuallyDeactivated { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiryAt;
    public bool IsActive => !IsExpired;


    private EmailVerificationCode() { }


    public static EmailVerificationCode Create(string email, string hashedCode)
        => new()
        {
            Id = Guid.NewGuid(),
            Email = email,
            HashedCode = hashedCode,
            CreatedAt = DateTime.UtcNow,
            ExpiryAt = DateTime.UtcNow.AddMinutes(10),

            IsManuallyDeactivated = false
        };


    public void DeactivateManually()
    {
        if (!IsManuallyDeactivated)
        {
            IsManuallyDeactivated = true;
        }
    }

    public void UseCode()
    {
        if (!IsUsed)
        {
            IsUsed = true;
        }
    }
}
