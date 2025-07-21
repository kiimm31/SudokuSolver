using SixLabors.ImageSharp;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Interfaces;

/// <summary>
/// Defines the contract for image processing operations
/// </summary>
public interface IImageProcessor
{
    /// <summary>
    /// Validates if the provided image stream is in a supported format
    /// </summary>
    /// <param name="imageStream">The image stream to validate</param>
    /// <returns>True if the format is supported, false otherwise</returns>
    Task<bool> ValidateFormatAsync(Stream imageStream);

    /// <summary>
    /// Processes an image for OCR optimization
    /// </summary>
    /// <param name="imageStream">The input image stream</param>
    /// <param name="options">Processing options</param>
    /// <returns>The processed image optimized for OCR</returns>
    Task<Image> PreprocessImageAsync(Stream imageStream, ImageProcessingOptions options);

    /// <summary>
    /// Resizes an image to optimal dimensions for processing
    /// </summary>
    /// <param name="image">The image to resize</param>
    /// <param name="maxDimension">Maximum dimension (width or height)</param>
    /// <returns>The resized image</returns>
    Image ResizeImage(Image image, int maxDimension);

    /// <summary>
    /// Converts an image to grayscale
    /// </summary>
    /// <param name="image">The image to convert</param>
    /// <returns>The grayscale image</returns>
    Image ConvertToGrayscale(Image image);

    /// <summary>
    /// Enhances contrast of an image
    /// </summary>
    /// <param name="image">The image to enhance</param>
    /// <param name="contrastFactor">Contrast enhancement factor</param>
    /// <returns>The contrast-enhanced image</returns>
    Image EnhanceContrast(Image image, float contrastFactor);

    /// <summary>
    /// Reduces noise in an image
    /// </summary>
    /// <param name="image">The image to denoise</param>
    /// <returns>The denoised image</returns>
    Image ReduceNoise(Image image);
} 