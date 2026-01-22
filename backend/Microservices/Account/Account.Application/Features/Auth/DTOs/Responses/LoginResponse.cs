namespace Account.Application.Features.Auth.DTOs.Responses;

public record LoginResponse(string JwtToken,
                            int UserId);
