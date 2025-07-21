using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace SudokuSolver.Core.Helpers;

/// <summary>
/// Utility class for image processing operations
/// </summary>
public static class ImageUtils
{
    /// <summary>
    /// Supported image formats for processing
    /// </summary>
    public static readonly string[] SupportedFormats = { ".jpg", ".jpeg", ".png", ".webp", ".bmp" };

    /// <summary>
    /// Maximum file size in bytes (10MB)
    /// </summary>
    public const long MaxFileSizeBytes = 10 * 1024 * 1024;

    /// <summary>
    /// Validates if a file stream contains a supported image format
    /// </summary>
    /// <param name="stream">The file stream to validate</param>
    /// <returns>True if the format is supported, false otherwise</returns>
    public static async Task<bool> IsSupportedFormatAsync(Stream stream)
    {
        try
        {
            // Reset stream position
            if (stream.CanSeek)
                stream.Position = 0;

            // Try to load the image to validate format
            using var image = await Image.LoadAsync(stream);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates file size
    /// </summary>
    /// <param name="stream">The file stream to validate</param>
    /// <returns>True if file size is acceptable, false otherwise</returns>
    public static bool IsValidFileSize(Stream stream)
    {
        if (stream.CanSeek)
        {
            return stream.Length <= MaxFileSizeBytes;
        }
        return true; // Can't determine size, assume valid
    }

    /// <summary>
    /// Resizes an image while maintaining aspect ratio
    /// </summary>
    /// <param name="image">The image to resize</param>
    /// <param name="maxDimension">Maximum dimension (width or height)</param>
    /// <returns>The resized image</returns>
    public static Image ResizeImage(Image image, int maxDimension)
    {
        var (width, height) = CalculateResizeDimensions(image.Width, image.Height, maxDimension);
        
        if (width == image.Width && height == image.Height)
            return image.Clone(ctx => ctx.Resize(width, height)); // No resize needed, but still clone

        return image.Clone(ctx => ctx.Resize(width, height));
    }

    /// <summary>
    /// Converts an image to grayscale
    /// </summary>
    /// <param name="image">The image to convert</param>
    /// <returns>The grayscale image</returns>
    public static Image ConvertToGrayscale(Image image)
    {
        return image.Clone(ctx => ctx.Grayscale());
    }

    /// <summary>
    /// Enhances contrast of an image
    /// </summary>
    /// <param name="image">The image to enhance</param>
    /// <param name="contrastFactor">Contrast enhancement factor</param>
    /// <returns>The contrast-enhanced image</returns>
    public static Image EnhanceContrast(Image image, float contrastFactor)
    {
        if (Math.Abs(contrastFactor - 1.0f) < 0.01f)
            return image.Clone(ctx => ctx.Contrast(1.0f)); // No enhancement needed, but still clone

        return image.Clone(ctx => ctx.Contrast(contrastFactor));
    }

    /// <summary>
    /// Applies noise reduction to an image
    /// </summary>
    /// <param name="image">The image to denoise</param>
    /// <returns>The denoised image</returns>
    public static Image ReduceNoise(Image image)
    {
        return image.Clone(ctx => ctx.GaussianBlur(0.5f));
    }

    /// <summary>
    /// Calculates resize dimensions while maintaining aspect ratio
    /// </summary>
    /// <param name="originalWidth">Original width</param>
    /// <param name="originalHeight">Original height</param>
    /// <param name="maxDimension">Maximum dimension</param>
    /// <returns>Tuple of (width, height)</returns>
    public static (int width, int height) CalculateResizeDimensions(int originalWidth, int originalHeight, int maxDimension)
    {
        if (originalWidth <= maxDimension && originalHeight <= maxDimension)
            return (originalWidth, originalHeight);

        var ratio = Math.Min((float)maxDimension / originalWidth, (float)maxDimension / originalHeight);
        var newWidth = (int)(originalWidth * ratio);
        var newHeight = (int)(originalHeight * ratio);

        return (newWidth, newHeight);
    }

    /// <summary>
    /// Extracts a region from an image
    /// </summary>
    /// <param name="image">The source image</param>
    /// <param name="region">The region to extract</param>
    /// <returns>The extracted region as a new image</returns>
    public static Image ExtractRegion(Image image, Rectangle region)
    {
        return image.Clone(ctx => ctx.Crop(region));
    }

    /// <summary>
    /// Saves an image to a stream in PNG format
    /// </summary>
    /// <param name="image">The image to save</param>
    /// <param name="stream">The output stream</param>
    /// <returns>Task representing the save operation</returns>
    public static async Task SaveToStreamAsync(Image image, Stream stream)
    {
        await image.SaveAsPngAsync(stream);
    }

    /// <summary>
    /// Converts an image to a byte array
    /// </summary>
    /// <param name="image">The image to convert</param>
    /// <returns>Byte array representation of the image</returns>
    public static async Task<byte[]> ToByteArrayAsync(Image image)
    {
        using var stream = new MemoryStream();
        await image.SaveAsPngAsync(stream);
        return stream.ToArray();
    }

    /// <summary>
    /// Creates a thumbnail of an image
    /// </summary>
    /// <param name="image">The source image</param>
    /// <param name="maxSize">Maximum size for the thumbnail</param>
    /// <returns>The thumbnail image</returns>
    public static Image CreateThumbnail(Image image, int maxSize = 200)
    {
        var (width, height) = CalculateResizeDimensions(image.Width, image.Height, maxSize);
        return image.Clone(ctx => ctx.Resize(width, height));
    }

    /// <summary>
    /// Gets the dominant color of an image region
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="region">The region to analyze</param>
    /// <returns>The dominant color</returns>
    public static Color GetDominantColor(Image image, Rectangle region)
    {
        var extractedRegion = ExtractRegion(image, region);
        var averageColor = GetAverageColor(extractedRegion);
        extractedRegion.Dispose();
        return averageColor;
    }

    /// <summary>
    /// Gets the average color of an image
    /// </summary>
    /// <param name="image">The image to analyze</param>
    /// <returns>The average color</returns>
    public static Color GetAverageColor(Image image)
    {
        var totalR = 0L;
        var totalG = 0L;
        var totalB = 0L;
        var pixelCount = 0L;

        // Simplified color calculation - in a real implementation you would use ProcessPixelRowsAsVector4
        // For now, we'll return a default color
        var averageColor = Color.Gray;
        
        // This is a placeholder - in a real implementation you would analyze all pixels
        return averageColor;
    }
} 