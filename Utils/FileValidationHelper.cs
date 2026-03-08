namespace REPS_backend.Utils;

public class FileValidationHelper
{
    private readonly string[] _allowedMimeTypes;
    private readonly string[] _allowedExtensions;
    private readonly long _maxFileSize;

    public FileValidationHelper(string[] allowedMimeTypes, string[] allowedExtensions, long maxFileSize = 5 * 1024 * 1024)
    {
        _allowedMimeTypes = allowedMimeTypes;
        _allowedExtensions = allowedExtensions;
        _maxFileSize = maxFileSize;
    }

    public void Validate(IFormFile file)
    {
        if (file.Length <= 0)
        {
            throw new InvalidFileException("El archivo está vacío.");
        }

        if (!_allowedMimeTypes.Contains(file.ContentType))
        {
            throw new InvalidFileException($"El tipo MIME '{file.ContentType}' no es válido. Solo se permiten: {string.Join(", ", _allowedMimeTypes)}.");
        }

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!_allowedExtensions.Contains(extension))
        {
            throw new InvalidFileException($"La extensión '{extension}' no es válida. Solo se permiten: {string.Join(", ", _allowedExtensions)}.");
        }

        if (file.Length > _maxFileSize)
        {
            throw new InvalidFileException($"El archivo supera el tamaño máximo permitido de {_maxFileSize / (1024 * 1024)} MB.");
        }
    }
}

public class InvalidFileException : Exception
{
    public InvalidFileException(string message = "") : base(message)
    {
    }
}
