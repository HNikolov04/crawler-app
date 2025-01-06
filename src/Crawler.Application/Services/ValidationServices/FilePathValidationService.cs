using System.IO;

namespace Crawler.Application.Services.ValidationServices;

public class FilePathValidationService
{
    public bool IsValidFilePath(string filePath)
    {
        return File.Exists(filePath);
    }

    public void EnsureValidFilePath(string filePath)
    {
        if (!IsValidFilePath(filePath))
        {
            throw new FileNotFoundException($"The specified file could not be found: {filePath}");
        }
    }
}