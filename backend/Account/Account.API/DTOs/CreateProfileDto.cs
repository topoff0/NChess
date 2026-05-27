namespace Account.API.DTOs;


public record CreateProfileDto(IFormFile? ProfileImage,
                               string Username);
