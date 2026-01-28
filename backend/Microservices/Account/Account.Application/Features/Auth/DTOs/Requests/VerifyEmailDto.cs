namespace Account.Application.Features.Auth.DTOs.Requests;

public record VerifyEmailDto(string Email, string VerificationCode);
