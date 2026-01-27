namespace Account.Core.Entities;

public enum AuthProvider
{
    Email = 1,
    Google = 2,
}

public enum UserStatus
{
    Pending,
    Active
}

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public AuthProvider Provider { get; private set; }
    public UserStatus Status { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty;
    public string ImagePath { get; private set; } = string.Empty;
    public List<RefreshToken> RefreshTokens { get; private set; } = [];
    // TODO: Make cleaning mechanism for old tokens

    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }


    private User() { }

    public static User CreatePending(string email, AuthProvider provider)
        => new()
        {
            Id = Guid.NewGuid(),
            Email = email,
            Provider = provider,
            Status = UserStatus.Pending
        };


    public void SetUser(string username, string passwordHash, string imagePath)
    {
        PasswordHash = passwordHash;
        Username = username;
        ImagePath = imagePath;
        Status = UserStatus.Active;
        CreatedAt = DateTime.UtcNow;
        LastLoginAt = DateTime.UtcNow;
    }
}
