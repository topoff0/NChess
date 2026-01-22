namespace Account.Application.Features.Auth.DTOs.Requests;

public record LoginByPasswordRequest(string Email,
                                     string Password);
