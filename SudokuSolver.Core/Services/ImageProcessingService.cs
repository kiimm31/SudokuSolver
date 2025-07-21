using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SudokuSolver.Core.Helpers;
using SudokuSolver.Core.Interfaces;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Services;

/// <summary>
/// Service for processing and preprocessing images for Sudoku grid detection
/// </summary>
public class ImageProcessingService : IImageProcessor
{
    private readonly ILogger<ImageProcessingService> _logger;

    public ImageProcessingService(ILogger<ImageProcessingService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Validates if the image format is supported
    /// </summary>
    /// <param name="imageStream">The image stream to validate</param>
    /// <returns>True if the format is supported, false otherwise</returns>
    public async Task<bool> ValidateFormatAsync(Stream imageStream)
    {
        try
        {
            _logger.LogDebug("Validating image format");

            var isValid = await ImageUtils.IsSupportedFormatAsync(imageStream);
            
            _logger.LogDebug("Image format validation result: {IsValid}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating image format");
            return false;
        }
    }

    /// <summary>
    /// Processes an image for OCR optimization
    /// </summary>
    /// <param name="imageStream">The input image stream</param>
    /// <param name="options">Processing options</param>
    /// <returns>The processed image optimized for OCR</returns>
    public async Task<Image> PreprocessImageAsync(Stream imageStream, ImageProcessingOptions options)
    {
        return await ProcessImageAsync(imageStream, options);
    }

    /// <summary>
    /// Processes an image for Sudoku grid detection
    /// </summary>
    /// <param name="imageStream">The image stream to process</param>
    /// <param name="options">Processing options</param>
    /// <returns>The processed image</returns>
    public async Task<Image> ProcessImageAsync(Stream imageStream, ImageProcessingOptions options)
    {
        try
        {
            _logger.LogInformation("Starting image preprocessing with options: {@Options}", options);

            // Load the original image
            _logger.LogDebug("Loading image from stream...");
            var originalImage = await Image.LoadAsync(imageStream);
            _logger.LogDebug("Loaded image with dimensions: {Width}x{Height}", originalImage.Width, originalImage.Height);

            // Apply preprocessing pipeline
            _logger.LogDebug("Calculating resize dimensions...");
            var (newWidth, newHeight) = ImageUtils.CalculateResizeDimensions(originalImage.Width, originalImage.Height, options.MaxDimension);
            _logger.LogDebug("Resize dimensions: {NewWidth}x{NewHeight}", newWidth, newHeight);
            
            _logger.LogDebug("Applying image transformations...");
            var processedImage = originalImage.Clone(ctx => ctx
                .Resize(newWidth, newHeight)
                .Grayscale()
                .Contrast(options.ContrastFactor));

            _logger.LogInformation("Image preprocessing completed successfully");
            return processedImage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during image preprocessing: {Message}", ex.Message);
            throw new InvalidOperationException("Failed to preprocess image", ex);
        }
    }

    /// <summary>
    /// Resizes an image to optimal dimensions for processing
    /// </summary>
    /// <param name="image">The image to resize</param>
    /// <param name="maxDimension">Maximum dimension (width or height)</param>
    /// <returns>The resized image</returns>
    public Image ResizeImage(Image image, int maxDimension)
    {
        try
        {
            _logger.LogDebug("Resizing image from {OriginalWidth}x{OriginalHeight} to max dimension {MaxDimension}", 
                image.Width, image.Height, maxDimension);

            var (newWidth, newHeight) = ImageUtils.CalculateResizeDimensions(image.Width, image.Height, maxDimension);
            
            var resizedImage = image.Clone(ctx => ctx.Resize(newWidth, newHeight));
            
            _logger.LogDebug("Image resized to {NewWidth}x{NewHeight}", newWidth, newHeight);
            return resizedImage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resizing image");
            throw new InvalidOperationException("Failed to resize image", ex);
        }
    }

    /// <summary>
    /// Converts an image to grayscale
    /// </summary>
    /// <param name="image">The image to convert</param>
    /// <returns>The grayscale image</returns>
    public Image ConvertToGrayscale(Image image)
    {
        try
        {
            _logger.LogDebug("Converting image to grayscale");
            
            var grayscaleImage = image.Clone(ctx => ctx.Grayscale());
            
            _logger.LogDebug("Image converted to grayscale successfully");
            return grayscaleImage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting image to grayscale");
            throw new InvalidOperationException("Failed to convert image to grayscale", ex);
        }
    }

    /// <summary>
    /// Enhances contrast of an image
    /// </summary>
    /// <param name="image">The image to enhance</param>
    /// <param name="contrastFactor">Contrast enhancement factor</param>
    /// <returns>The contrast-enhanced image</returns>
    public Image EnhanceContrast(Image image, float contrastFactor)
    {
        try
        {
            _logger.LogDebug("Enhancing image contrast with factor: {ContrastFactor}", contrastFactor);
            
            var enhancedImage = image.Clone(ctx => ctx.Contrast(contrastFactor));
            
            _logger.LogDebug("Image contrast enhanced successfully");
            return enhancedImage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enhancing image contrast");
            throw new InvalidOperationException("Failed to enhance image contrast", ex);
        }
    }

    /// <summary>
    /// Reduces noise in an image
    /// </summary>
    /// <param name="image">The image to denoise</param>
    /// <param name="strength">Denoising strength</param>
    /// <returns>The denoised image</returns>
    public Image DenoiseImage(Image image, float strength)
    {
        try
        {
            _logger.LogDebug("Denoising image with strength: {Strength}", strength);
            
            var denoisedImage = image.Clone(ctx => ctx.GaussianBlur(strength));
            
            _logger.LogDebug("Image denoised successfully");
            return denoisedImage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error denoising image");
            throw new InvalidOperationException("Failed to denoise image", ex);
        }
    }

    /// <summary>
    /// Reduces noise in an image using default settings
    /// </summary>
    /// <param name="image">The image to denoise</param>
    /// <returns>The denoised image</returns>
    public Image ReduceNoise(Image image)
    {
        try
        {
            _logger.LogDebug("Reducing noise in image");
            
            var denoisedImage = image.Clone(ctx => ctx.GaussianBlur(0.5f));
            
            _logger.LogDebug("Image noise reduction completed successfully");
            return denoisedImage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reducing noise in image");
            throw new InvalidOperationException("Failed to reduce noise in image", ex);
        }
    }

    /// <summary>
    /// Sharpens an image
    /// </summary>
    /// <param name="image">The image to sharpen</param>
    /// <param name="strength">Sharpening strength</param>
    /// <returns>The sharpened image</returns>
    public Image SharpenImage(Image image, float strength)
    {
        try
        {
            _logger.LogDebug("Sharpening image with strength: {Strength}", strength);
            
            var sharpenedImage = image.Clone(ctx => ctx
                .GaussianBlur(0.5f)
                .Contrast(1.2f));
            
            _logger.LogDebug("Image sharpened successfully");
            return sharpenedImage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sharpening image");
            throw new InvalidOperationException("Failed to sharpen image", ex);
        }
    }
} 