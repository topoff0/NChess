namespace Account.Application.Features.Auth.DTOs.Requests;

public record GenerateAccessTokenRequest(int UserId,
                                         string Username,
                                         string Email);
