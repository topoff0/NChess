namespace Account.Application.Common.Configurations;

public sealed class JwtOptions
{
    public const string JwtOptionsKey = "JwtOptions";

    public required string Secret { get; init; }
    public required string Audience { get; init; } 
    public required string Issuer { get; init; } 
    public required int AccessTokenExpiryMinutes { get; init; }
    public required int RefreshTokenExpriryDays { get; init; }
}
