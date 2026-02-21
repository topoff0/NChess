namespace Account.Application.Common.Errors;

public enum ErrorType
{
    Failure = 0,
    NotFound = 1,
    Validation = 2,
    Conflict = 3,
    AccessUnAuthorized = 4,
    AccessForbidden = 5
}

public class Error
{
    private Error(
        string code,
        string description,
        ErrorType errorType,
        Dictionary<string, string[]>? validationErrors = null
    )
    {
        Code = code;
        Description = description;
        ErrorType = errorType;
        ValidationErrors = validationErrors;
    }

    public string Code { get; }

    public string Description { get; }

    public ErrorType ErrorType { get; }

    public Dictionary<string, string[]>? ValidationErrors { get; }

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Validation(Dictionary<string, string[]> validationErrors) =>
        new(ErrorCodes.GeneralValidation, "One or more validation errors occurred", ErrorType.Validation, validationErrors);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error AccessUnAuthorized(string code, string description) =>
        new(code, description, ErrorType.AccessUnAuthorized);

    public static Error AccessForbidden(string code, string description) =>
        new(code, description, ErrorType.AccessForbidden);
}
