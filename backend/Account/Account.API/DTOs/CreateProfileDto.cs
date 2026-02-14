namespace Account.API.DTOs;


public record CreateProfileDto(IFormFile? ProfileImage,
                               string Email,
                               string Username,
                               string Password,
                               string ConfirmPassword);
