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

public sealed class User
{
    private const int MaxActiveRefreshToken = 5;

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty;
    public string ImagePath { get; private set; } = string.Empty;

    public AuthProvider Provider { get; private set; }
    public UserStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public List<RefreshToken> RefreshTokens { get; private set; } = [];


    private User() { }


    public static User CreatePending(string email, AuthProvider provider)
        => new()
        {
            Id = Guid.NewGuid(),
            Email = email,
            Provider = provider,
            Status = UserStatus.Pending
        };

    public void Activate(string username, string imagePath)
    {
        Username = username;
        ImagePath = imagePath;
        Status = UserStatus.Active;
        CreatedAt = DateTime.UtcNow;
        LastLoginAt = DateTime.UtcNow;
    }

    public void UpdateLastLoginTime()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public RefreshToken CreateRefreshToken(int expiryDays)
    {
        CleanupOldTokens();

        var activeTokens = RefreshTokens.Where(t => t.IsActive).ToList();

        if (activeTokens.Count >= MaxActiveRefreshToken)
        {
            activeTokens
                .OrderBy(t => t.CreatedAt)
                .First()
                .Revoke();
        }

        var refreshToken = RefreshToken.Create(Id, expiryDays);
        RefreshTokens.Add(refreshToken);

        return refreshToken;
    }

    public RefreshToken RotateRefreshToken(string token, int expiryDays)
    {
        var existingToken = RefreshTokens.FirstOrDefault(t => t.TokenHash == token)
            ?? throw new ArgumentException("Refresh token not found");

        if (!existingToken.IsActive)
            throw new ArgumentException("Refresh token is not active");

        var newRefreshToken = RefreshToken.Create(Id, expiryDays);

        existingToken.Revoke(newRefreshToken.TokenHash);
        RefreshTokens.Add(newRefreshToken);

        return newRefreshToken;
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var token in RefreshTokens.Where(t => t.IsActive))
        {
            token.Revoke();
        }
    }


    private void CleanupOldTokens()
    {
        RefreshTokens.RemoveAll(t => !t.IsActive && t.RevokedAt != null);
    }
}
