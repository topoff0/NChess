namespace Account.Application.Features.Auth.DTOs.Requests;

public record CreateAccountRequest(string Username,
                                   string Password,
                                   string ConfirmPassword,
                                   byte[]? ProfileImageBytes = null,
                                   string? ProfileImagePath = null);
