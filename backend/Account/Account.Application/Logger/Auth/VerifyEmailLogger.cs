using Microsoft.Extensions.Logging;

namespace Account.Application.Logger.Auth;

public static partial class VerifyEmailLogger
{
    [LoggerMessage(
        EventId = 1100,
        Level = LogLevel.Information,
        Message = "Start verify email command")]
    public static partial void LogStartVerifyEmail(this ILogger logger);

    [LoggerMessage(
        EventId = 1101,
        Level = LogLevel.Warning,
        Message = "User with email '{Email}' not found")]
    public static partial void LogUserWithSuchEmailNotFoundVerifyEmail(this ILogger logger, string email);


    [LoggerMessage(
        EventId = 1102,
        Level = LogLevel.Warning,
        Message = "Verification code for email '{Email}' not found")]
    public static partial void LogVerificationCodeForThisEmailNotFoundVerify(this ILogger logger, string email);

    [LoggerMessage(
        EventId = 1103,
        Level = LogLevel.Warning,
        Message = "Verification code '{Code}' for email '{Email}' has already used")]
    public static partial void LogVerificationCodeAlreadyUsed(this ILogger logger, string code, string email);

    [LoggerMessage(
        EventId = 1104,
        Level = LogLevel.Warning,
        Message = "Verification code '{Code}' for email '{Email}' is not correct")]
    public static partial void LogVerificationCodeForThisEmailNotCorrect(this ILogger logger, string code, string email);

    [LoggerMessage(
        EventId = 1105,
        Level = LogLevel.Information,
        Message = "Successful email verification; Email: '{Email}', Code: '{Code}'")]
    public static partial void LogSuccessfulEmailVerification(this ILogger logger, string code, string email);

    [LoggerMessage(
        EventId = 1199,
        Level = LogLevel.Critical,
        Message = "An unexpected error occurred while processing verify email command: {Message}")]
    public static partial void LogUnexpectedErrorStartEmailAuth(this ILogger logger, string message);
}
