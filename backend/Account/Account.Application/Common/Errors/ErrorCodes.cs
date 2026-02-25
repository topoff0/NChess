namespace Account.Application.Common.Errors;

public static class ErrorCodes
{
    // Auth errors
    public const string InvalidVerificationCode = "auth.invalid_verification_code";
    public const string InvalidPassword = "auth.incorrect_password";

    public const string UserNotFound = "auth.user_not_found";
    public const string VerificationCodeNotFound = "auth.verification_code_not_found";

    public const string InvalidToken = "auth.invalid_token";
    public const string TokenExpired = "auth.token_expired";
    public const string AccountNotActivated = "auth.account_not_activated";
    public const string VerificationCodeAlreadyUsed = "auth.verification_code_already_used";

    public const string UsernameAlreadyExists = "auth.username_already_exists";
    public const string ProfileAlreadyCreated = "auth.profile_already_created";

    // Validation errors
    public const string GeneralValidation = "validation_error";
    public const string ModelStateValidationFailed = "model_state_validation.failed";

    // System errors
    public const string InternalError = "system.internal_error";
    public const string ServiceUnavailable = "system.service_unavailable";

    // Unexpected errors
    public const string AuthUnexpectedError = "auth.unexpected_error";
}

public static class ErrorMessages
{
    // Auth messages
    public const string InvalidVerificationCode = "Invalid verification code";
    public const string InvalidPassword = "User with this email and password not found";

    public const string UserNotFound = "User is not found";
    public const string VerificationCodeNotFound = "We have not sent any verification codes to this email";

    public const string AccountNotActivated = "User is not activated";
    public const string VerficaitonCodeAlreadyUsed = "Someone has already used your code";

    // Validation messages
    public const string ModelStateValidationFailed = "Invalid request data format";

    public const string UsernameAlreadyExists = "Username is already taken";
    public const string ProfileAlreadyCreated = "Profile has already been created";

    // System messages
    public const string InternalError = "An unexpected error occurred";

    // Unexpected messages
    public const string AuthUnexpectedError = "Something went wrong";
}
