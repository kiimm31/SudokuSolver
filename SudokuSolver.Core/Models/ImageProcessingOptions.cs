namespace SudokuSolver.Core.Models;

/// <summary>
/// Configuration options for image processing
/// </summary>
public class ImageProcessingOptions
{
    /// <summary>
    /// Maximum dimension (width or height) for image resizing
    /// </summary>
    public int MaxDimension { get; set; } = 2048;

    /// <summary>
    /// Contrast enhancement factor (1.0 = no change, 2.0 = double contrast)
    /// </summary>
    public float ContrastFactor { get; set; } = 1.5f;

    /// <summary>
    /// Whether to apply noise reduction
    /// </summary>
    public bool ApplyNoiseReduction { get; set; } = true;

    /// <summary>
    /// Whether to apply contrast enhancement
    /// </summary>
    public bool ApplyContrastEnhancement { get; set; } = true;

    /// <summary>
    /// Whether to convert to grayscale
    /// </summary>
    public bool ConvertToGrayscale { get; set; } = true;

    /// <summary>
    /// OCR confidence threshold (0.0 to 1.0)
    /// </summary>
    public float ConfidenceThreshold { get; set; } = 0.7f;

    /// <summary>
    /// Whether to enable debug mode (saves intermediate images)
    /// </summary>
    public bool DebugMode { get; set; } = false;

    /// <summary>
    /// Path to save debug images (if debug mode is enabled)
    /// </summary>
    public string? DebugOutputPath { get; set; }

    /// <summary>
    /// Creates default processing options
    /// </summary>
    /// <returns>Default image processing options</returns>
    public static ImageProcessingOptions Default => new()
    {
        MaxDimension = 2048,
        ContrastFactor = 1.5f,
        ApplyNoiseReduction = true,
        ApplyContrastEnhancement = true,
        ConvertToGrayscale = true,
        ConfidenceThreshold = 0.7f,
        DebugMode = false
    };

    /// <summary>
    /// Creates high-quality processing options (slower but more accurate)
    /// </summary>
    /// <returns>High-quality image processing options</returns>
    public static ImageProcessingOptions HighQuality => new()
    {
        MaxDimension = 4096,
        ContrastFactor = 2.0f,
        ApplyNoiseReduction = true,
        ApplyContrastEnhancement = true,
        ConvertToGrayscale = true,
        ConfidenceThreshold = 0.8f,
        DebugMode = false
    };

    /// <summary>
    /// Creates fast processing options (faster but less accurate)
    /// </summary>
    /// <returns>Fast image processing options</returns>
    public static ImageProcessingOptions Fast => new()
    {
        MaxDimension = 1024,
        ContrastFactor = 1.2f,
        ApplyNoiseReduction = false,
        ApplyContrastEnhancement = true,
        ConvertToGrayscale = true,
        ConfidenceThreshold = 0.6f,
        DebugMode = false
    };
} 