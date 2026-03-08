using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using REPS_backend.Configurations;
using REPS_backend.Utils;

namespace REPS_backend.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string?> UploadImageAsync(IFormFile file)
    {
        var imageValidator = new FileValidationHelper(
            new[] { "image/jpeg", "image/png", "image/webp", "image/gif" },
            new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" }
        );
        imageValidator.Validate(file);

        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Width(800).Height(600).Crop("fill"),
            Folder = "reps/rutinas"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new Exception($"Cloudinary Error: {uploadResult.Error.Message}");
        }

        return uploadResult.SecureUrl?.ToString();
    }

    public async Task<string?> UploadAvatarAsync(IFormFile file)
    {
        var imageValidator = new FileValidationHelper(
            new[] { "image/jpeg", "image/png", "image/webp", "image/gif" },
            new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" }
        );
        imageValidator.Validate(file);

        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Width(300).Height(300).Crop("fill").Gravity("face"),
            Folder = "reps/avatars"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new Exception($"Cloudinary Error: {uploadResult.Error.Message}");
        }

        return uploadResult.SecureUrl?.ToString();
    }

    public async Task<string?> UploadVideoAsync(IFormFile file)
    {
        var videoValidator = new FileValidationHelper(
            new[] { "video/mp4", "video/mpeg", "video/quicktime" },
            new[] { ".mp4", ".mpeg", ".mov" },
            maxFileSize: 50 * 1024 * 1024 // 50MB for videos
        );
        videoValidator.Validate(file);

        using var stream = file.OpenReadStream();
        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "reps/ejercicios"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        
        if (uploadResult.Error != null)
        {
            throw new Exception($"Cloudinary Error: {uploadResult.Error.Message}");
        }

        return uploadResult.SecureUrl?.ToString();
    }

    public async Task DeleteImageAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image
        };
        await _cloudinary.DestroyAsync(deletionParams);
    }

    public async Task DeleteVideoAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Video
        };
        await _cloudinary.DestroyAsync(deletionParams);
    }
}
