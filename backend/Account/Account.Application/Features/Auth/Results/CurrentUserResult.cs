namespace Account.Application.Features.Auth.Results;

public record CurrentUserResult(
        Guid Id,
        string Email,
        string? Username,
        string ImagePath,
        bool IsProfileCreated);
