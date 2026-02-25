namespace Account.Application.Interfaces;

public interface IImageService
{
    Task<string> SaveProfileImageAsync(byte[] imageBytes, CancellationToken token = default);
}
