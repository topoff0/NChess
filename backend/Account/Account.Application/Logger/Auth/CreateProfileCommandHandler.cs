using Microsoft.Extensions.Logging;

namespace Account.Application.Logger.Auth;

public static partial class CreateProfileCommandHandler
{
    [LoggerMessage(
        EventId = 1300,
        Level = LogLevel.Information,
        Message = "Start creating profile")]
    public static partial void LogStartCreateProfile(this ILogger logger);

    [LoggerMessage(
        EventId = 1301,
        Level = LogLevel.Warning,
        Message = "User with email '{Email}' not found")]
    public static partial void LogUserWithSuchEmailNotFoundCreateProfile(this ILogger logger, string email);

    [LoggerMessage(
        EventId = 1302,
        Level = LogLevel.Information,
        Message = "Successful create account")]
    public static partial void LogSuccessfulCreateAccount(this ILogger logger);

    [LoggerMessage(
        EventId = 1399,
        Level = LogLevel.Critical,
        Message = "An unexpected error occurred while processing create profile command: {Message}")]
    public static partial void LogUnexpectedErrorCreateProfile(this ILogger logger, string message);
}
