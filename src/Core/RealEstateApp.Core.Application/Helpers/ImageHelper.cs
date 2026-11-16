using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Core.Application.Helpers;

public static class ImageHelper
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

    public static bool IsValidImage(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    /// <summary>
    /// Guarda la imagen en wwwroot/Images/{folder} y devuelve la ruta relativa (ej. /Images/Properties/abc.png).
    /// Si se pasa existingPath, la imagen anterior se elimina (edición).
    /// </summary>
    public static async Task<string?> UploadAsync(IFormFile? file, string webRootPath, string folder, string? existingPath = null)
    {
        if (!IsValidImage(file))
        {
            return existingPath;
        }

        var directory = Path.Combine(webRootPath, "Images", folder);
        Directory.CreateDirectory(directory);

        var extension = Path.GetExtension(file!.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(directory, fileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        Delete(existingPath, webRootPath);

        return $"/Images/{folder}/{fileName}";
    }

    public static void Delete(string? relativePath, string webRootPath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        var fullPath = Path.Combine(webRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
