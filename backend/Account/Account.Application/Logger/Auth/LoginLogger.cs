using Microsoft.Extensions.Logging;

namespace Account.Application.Logger.Auth;

public static partial class LoginLogger
{
    [LoggerMessage(
        EventId = 1200,
        Level = LogLevel.Information,
        Message = "Start of the login process")]
    public static partial void LogStartLogin(this ILogger logger);

    [LoggerMessage(
        EventId = 1201,
        Level = LogLevel.Warning,
        Message = "User with email '{Email}' not found")]
    public static partial void LogUserWithSuchEmailNotFoundLogin(this ILogger logger, string email);

    [LoggerMessage(
        EventId = 1202,
        Level = LogLevel.Warning,
        Message = "User with email '{Email}' not active for login")]
    public static partial void LogUserWithSuchEmailNotActiveLogin(this ILogger logger, string email);

    [LoggerMessage(
        EventId = 1203,
        Level = LogLevel.Warning,
        Message = "Incorrect password for user with email '{Email}'")]
    public static partial void LogIncorrectPasswordLogin(this ILogger logger, string email);

    [LoggerMessage(
        EventId = 1204,
        Level = LogLevel.Information,
        Message = "Successful login for user with email '{Email}'")]
    public static partial void LogSuccessfulLogin(this ILogger logger, string email);

    [LoggerMessage(
        EventId = 1299,
        Level = LogLevel.Critical,
        Message = "An unexpected error occurred while processing login command: {Message}")]
    public static partial void LogUnexpectedErrorLogin(this ILogger logger, string message);
}
