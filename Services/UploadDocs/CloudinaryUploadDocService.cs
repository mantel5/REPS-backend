using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using REPS_backend.Configurations;
using REPS_backend.Utils;

namespace REPS_backend.Services.UploadDocs;

public class CloudinaryUploadDocService : IUploadDocService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryUploadDocService(IConfiguration configuration)
    {
        var cloudName = configuration["CloudinarySettings:CloudName"];
        var apiKey = configuration["CloudinarySettings:ApiKey"];
        var apiSecret = configuration["CloudinarySettings:ApiSecret"];
        
        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string?> UploadImageAsync(IFormFile file)
    {
        var imageValidator = new FileValidationHelper(
            new[] { "image/jpeg", "image/png", "image/gif" },
            new[] { ".jpg", ".jpeg", ".png", ".gif" }
        );
        imageValidator.Validate(file);

        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.SecureUrl?.ToString();
    }

    public async Task<string?> UploadPDFAsync(IFormFile file)
    {
        var pdfValidator = new FileValidationHelper(
            new[] { "application/pdf" },
            new[] { ".pdf" }
        );
        pdfValidator.Validate(file);

        using var stream = file.OpenReadStream();
        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(file.FileName, stream)
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
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

    public async Task DeleteDocAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Raw
        };
        await _cloudinary.DestroyAsync(deletionParams);
    }
}
