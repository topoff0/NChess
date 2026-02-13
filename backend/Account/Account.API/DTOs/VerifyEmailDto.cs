namespace Account.API.DTOs;

public record VerifyEmailDto(string Email, string VerificationCode);
