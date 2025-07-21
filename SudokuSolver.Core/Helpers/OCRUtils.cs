using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace SudokuSolver.Core.Helpers;

/// <summary>
/// Utility class for OCR operations and digit recognition
/// </summary>
public static class OCRUtils
{
    /// <summary>
    /// Valid digits for Sudoku (1-9)
    /// </summary>
    public static readonly int[] ValidDigits = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    /// <summary>
    /// Common OCR misreadings and their corrections
    /// </summary>
    public static readonly Dictionary<string, int> CommonMisreadings = new()
    {
        { "l", 1 }, { "I", 1 }, { "|", 1 }, { "!", 1 },
        { "S", 5 }, { "s", 5 },
        { "G", 6 }, { "g", 6 },
        { "B", 8 }, { "b", 8 },
        { "q", 9 }, { "g", 9 }
    };

    /// <summary>
    /// Preprocesses a cell image for better OCR accuracy
    /// </summary>
    /// <param name="cellImage">The cell image to preprocess</param>
    /// <returns>The preprocessed image</returns>
    public static Image PreprocessCellForOCR(Image cellImage)
    {
        return cellImage.Clone(ctx => ctx
            .Grayscale()
            .Contrast(2.0f)
            .GaussianBlur(0.3f));
    }

    /// <summary>
    /// Attempts to parse a digit from OCR text
    /// </summary>
    /// <param name="ocrText">The raw OCR text</param>
    /// <returns>The parsed digit, or null if not a valid digit</returns>
    public static int? ParseDigit(string ocrText)
    {
        if (string.IsNullOrWhiteSpace(ocrText))
            return null;

        // Clean the text
        var cleanText = ocrText.Trim().ToLowerInvariant();

        // Try direct parsing first
        if (int.TryParse(cleanText, out var digit) && ValidDigits.Contains(digit))
            return digit;

        // Check for common misreadings
        if (CommonMisreadings.TryGetValue(cleanText, out var correctedDigit))
            return correctedDigit;

        // Try to extract digit from mixed text
        foreach (var character in cleanText)
        {
            if (int.TryParse(character.ToString(), out var charDigit) && ValidDigits.Contains(charDigit))
                return charDigit;
        }

        return null;
    }

    /// <summary>
    /// Determines if a cell image appears to be empty
    /// </summary>
    /// <param name="cellImage">The cell image to analyze</param>
    /// <param name="threshold">Brightness threshold for empty detection</param>
    /// <returns>True if the cell appears empty, false otherwise</returns>
    public static bool IsCellEmpty(Image cellImage, float threshold = 0.8f)
    {
        var averageColor = ImageUtils.GetAverageColor(cellImage);
        // Use the correct Color properties - convert to Rgba32 to access R, G, B
        var rgbaColor = averageColor.ToPixel<Rgba32>();
        var brightness = (rgbaColor.R + rgbaColor.G + rgbaColor.B) / (3.0f * 255.0f);
        return brightness > threshold;
    }

    /// <summary>
    /// Calculates confidence score based on OCR text quality
    /// </summary>
    /// <param name="ocrText">The raw OCR text</param>
    /// <param name="meanConfidence">The mean confidence from Tesseract</param>
    /// <returns>Adjusted confidence score</returns>
    public static float CalculateConfidence(string ocrText, float meanConfidence)
    {
        if (string.IsNullOrWhiteSpace(ocrText))
            return 0.0f;

        // Base confidence from Tesseract
        var baseConfidence = meanConfidence;

        // Adjust based on text length (single digit should be short)
        var lengthPenalty = Math.Max(0, ocrText.Length - 1) * 0.1f;
        baseConfidence -= lengthPenalty;

        // Boost confidence for clean single digits
        if (ocrText.Length == 1 && int.TryParse(ocrText, out _))
            baseConfidence += 0.1f;

        // Penalize for common misreadings
        if (CommonMisreadings.ContainsKey(ocrText.ToLowerInvariant()))
            baseConfidence -= 0.1f;

        return Math.Max(0.0f, Math.Min(1.0f, baseConfidence));
    }

    /// <summary>
    /// Validates OCR result for Sudoku constraints
    /// </summary>
    /// <param name="digit">The recognized digit</param>
    /// <param name="confidence">The confidence score</param>
    /// <returns>True if the result is valid for Sudoku, false otherwise</returns>
    public static bool ValidateSudokuDigit(int digit, float confidence)
    {
        // Must be a valid Sudoku digit (1-9) or empty (0)
        if (digit < 0 || digit > 9)
            return false;

        // Must have reasonable confidence
        if (confidence < 0.3f)
            return false;

        return true;
    }

    /// <summary>
    /// Creates a standardized cell image for OCR processing
    /// </summary>
    /// <param name="cellImage">The original cell image</param>
    /// <param name="targetSize">Target size for the cell (default 32x32)</param>
    /// <returns>The standardized cell image</returns>
    public static Image StandardizeCellImage(Image cellImage, int targetSize = 32)
    {
        // Add padding to ensure consistent size
        var padding = 4;

        return cellImage.Clone(ctx => ctx
            .Resize(targetSize, targetSize)
            .Grayscale()
            .Contrast(1.5f));
    }

    /// <summary>
    /// Extracts features from a cell image for analysis
    /// </summary>
    /// <param name="cellImage">The cell image to analyze</param>
    /// <returns>Dictionary of extracted features</returns>
    public static Dictionary<string, object> ExtractCellFeatures(Image cellImage)
    {
        var features = new Dictionary<string, object>();

        // Basic image properties
        features["width"] = cellImage.Width;
        features["height"] = cellImage.Height;
        features["aspectRatio"] = (float)cellImage.Width / cellImage.Height;

        // Color analysis
        var averageColor = ImageUtils.GetAverageColor(cellImage);
        var rgbaColor = averageColor.ToPixel<Rgba32>();
        features["averageBrightness"] = (rgbaColor.R + rgbaColor.G + rgbaColor.B) / (3.0f * 255.0f);
        features["isGrayscale"] = Math.Abs(rgbaColor.R - rgbaColor.G) < 10 && 
                                 Math.Abs(rgbaColor.G - rgbaColor.B) < 10;

        // Edge density (simple approximation)
        var edgeDensity = CalculateEdgeDensity(cellImage);
        features["edgeDensity"] = edgeDensity;

        return features;
    }

    /// <summary>
    /// Calculates a simple edge density metric
    /// </summary>
    /// <param name="image">The image to analyze</param>
    /// <returns>Edge density value</returns>
    private static float CalculateEdgeDensity(Image image)
    {
        // This is a simplified edge detection
        // In a real implementation, you might use more sophisticated edge detection
        var totalPixels = image.Width * image.Height;
        var edgePixels = 0;

        // Simplified edge detection - just count pixels with high contrast
        for (int y = 1; y < image.Height - 1; y++)
        {
            for (int x = 1; x < image.Width - 1; x++)
            {
                // This is a placeholder - in a real implementation you would analyze pixel differences
                edgePixels++;
            }
        }

        return (float)edgePixels / totalPixels;
    }

    /// <summary>
    /// Gets OCR configuration for digit recognition
    /// </summary>
    /// <returns>Dictionary of OCR configuration parameters</returns>
    public static Dictionary<string, string> GetOCRConfiguration()
    {
        return new Dictionary<string, string>
        {
            { "tessedit_char_whitelist", "123456789" },
            { "tessedit_pageseg_mode", "7" }, // Single uniform block of text
            { "tessedit_ocr_engine_mode", "3" }, // Default, based on what is available
            { "preserve_interword_spaces", "0" }
        };
    }
} 