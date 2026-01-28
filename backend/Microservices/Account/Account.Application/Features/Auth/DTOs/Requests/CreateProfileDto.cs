namespace Account.Application.Features.Auth.DTOs.Requests;


public record CreateProfileDto(byte[] ProfileImage,
                               string Email,
                               string Username,
                               string Password,
                               string ConfirmPassword);
