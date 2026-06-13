using Microsoft.Extensions.Logging;

namespace Account.Application.Auth.Logger;

public static partial class StartEmailAuthLogger
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "Start email authentication")]
    public static partial void LogStartEmailAuthentication(this ILogger logger);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Email validation error in start email authentication")]
    public static partial void LogEmailValidationError(this ILogger logger);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Information,
        Message = "User with email '{Email}' does not exists")]
    public static partial void LogUserNotExists(this ILogger logger, string email);

    [LoggerMessage(
        EventId = 1005,
        Level = LogLevel.Information,
        Message = "Successful start email authentication")]
    public static partial void LogSuccessfulStartEmailAuth(this ILogger logger);

    [LoggerMessage(
        EventId = 1099,
        Level = LogLevel.Critical,
        Message = "An unexpected error occurred while processing start email authentication command: {Message}")]
    public static partial void LogUnexpectedErrorStartEmailAuth(this ILogger logger, string message);
}
