namespace Account.Application.Features.Auth.DTOs.Requests;

public record VerifyCodeRequest(string Email,
                                string Code);
