namespace Account.Application.Features.Auth.DTOs.Responses;

public record VerifyCodeResponse(bool IsCodeCorrect,
                                 string? JwtToken);
