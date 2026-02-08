namespace Account.Application.Features.Auth.DTOs.Results;

public record IsUserExistsAndActiveResult(bool IsExists, bool IsActive);
