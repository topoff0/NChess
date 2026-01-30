namespace Account.Application.Common.Configurations;

public sealed class EmailOptions
{
    public const string EmailOptionsKey = "EmailOptions";

    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}
