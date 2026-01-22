namespace Account.Application.Features.Auth.DTOs.Responses;

public record CheckUserResponse(bool IsExists,
                                bool IsEmailEonfirmed,
                                bool IsAccountCreated);
