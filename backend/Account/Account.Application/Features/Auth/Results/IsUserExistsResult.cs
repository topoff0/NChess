namespace Account.Application.Features.Auth.Results;

public record IsUserExistsAndActiveResult(bool IsExists, bool IsActive);
