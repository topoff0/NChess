namespace Account.API.DTOs;


public record CreateProfileDto(byte[] ProfileImage,
                               string Email,
                               string Username,
                               string Password,
                               string ConfirmPassword);
