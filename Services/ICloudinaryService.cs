namespace REPS_backend.Services;

public interface ICloudinaryService
{
    Task<string?> UploadImageAsync(IFormFile file);
    Task<string?> UploadAvatarAsync(IFormFile file);
    Task<string?> UploadVideoAsync(IFormFile file);
    Task DeleteImageAsync(string publicId);
    Task DeleteVideoAsync(string publicId);
}
