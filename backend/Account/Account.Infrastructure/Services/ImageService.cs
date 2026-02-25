using Account.Application.Interfaces;

namespace Account.Infrastructure.Services;

public class ImageService : IImageService
{
    private static readonly Dictionary<byte, string> imageExtension = new()
    {
        [0xFF] = ".jpg",
        [0x89] = ".png",
        [0x47] = ".gif",
        [0x42] = ".bmp"
    };

    private const string baseDirectory = "wwwroot";

    public async Task<string> SaveProfileImageAsync(byte[] imageBytes, CancellationToken token = default)
    {
        if (imageBytes.Length == 0)
        {
            return string.Empty;
        }

        if (!imageExtension.TryGetValue(imageBytes[0], out var extension))
        {
            throw new InvalidOperationException("Unsupported image format");
        }

        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), baseDirectory, "profile-images");
        Directory.CreateDirectory(directoryPath);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(directoryPath, fileName);

        await File.WriteAllBytesAsync(filePath, imageBytes, token);

        return $"/profile-images/{fileName}";
    }
}
