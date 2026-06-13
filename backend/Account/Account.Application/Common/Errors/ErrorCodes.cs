namespace Account.Application.Common.Errors;

public static class ErrorCodes
{
    // Auth errors
    public const string InvalidVerificationCode = "auth.invalid_verification_code";
    public const string UserNotFound = "auth.user_not_found";
    public const string VerificationCodeNotFound = "auth.verification_code_not_found";
    public const string InvalidToken = "auth.invalid_token";
    public const string TokenExpired = "auth.token_expired";
    public const string VerificationCodeAlreadyUsed = "auth.verification_code_already_used";
    public const string UsernameAlreadyExists = "auth.username_already_exists";
    public const string ProfileAlreadyCreated = "auth.profile_already_created";

    // Validation errors
    public const string GeneralValidation = "validation_error";
    public const string ModelStateValidationFailed = "validation.invalid_model_state";
    public const string IncorrectImageFormat = "validation.incorrect_image_format";

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
    public const string UserNotFound = "User is not found";
    public const string VerificationCodeNotFound = "We have not sent any verification codes to this email";
    public const string VerificationCodeAlreadyUsed = "Someone has already used your code";
    public const string UsernameAlreadyExists = "Username is already taken";
    public const string ProfileAlreadyCreated = "Profile has already been created";

    // Validation messages
    public const string ModelStateValidationFailed = "Invalid request data format";
    public const string IncorrectImageFormat = "Incorrect image format";

    // System messages
    public const string InternalError = "An unexpected error occurred";

    // Unexpected messages
    public const string AuthUnexpectedError = "Something went wrong";
}
