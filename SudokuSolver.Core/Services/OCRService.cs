using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SudokuSolver.Core.Helpers;
using SudokuSolver.Core.Interfaces;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Services;

/// <summary>
/// Service for performing OCR (Optical Character Recognition) on Sudoku cell images
/// </summary>
public class OCRService : IOCRService, IDisposable
{
    private readonly ILogger<OCRService> _logger;
    private bool _isInitialized;
    private bool _disposed;

    public OCRService(ILogger<OCRService> logger)
    {
        _logger = logger;
        _isInitialized = false;
        _disposed = false;
    }

    /// <summary>
    /// Initializes the OCR service with Tesseract data
    /// </summary>
    /// <param name="tessdataPath">Path to Tesseract data files</param>
    /// <returns>True if initialization was successful</returns>
    public async Task<bool> InitializeAsync(string tessdataPath)
    {
        try
        {
            _logger.LogInformation("Initializing OCR service with tessdata path: {TessdataPath}", tessdataPath);

            // In a real implementation, this would initialize Tesseract
            // For now, we'll simulate successful initialization
            await Task.Delay(100); // Simulate initialization time
            
            _isInitialized = true;
            _logger.LogInformation("OCR service initialized successfully");
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize OCR service");
            return false;
        }
    }

    /// <summary>
    /// Recognizes digits in an array of cell images
    /// </summary>
    /// <param name="cellImages">Array of cell images to process</param>
    /// <returns>Array of OCR results</returns>
    public async Task<OCRResult[]> RecognizeDigitsAsync(Image[] cellImages)
    {
        return await RecognizeDigitsAsync(cellImages, GetConfidenceThreshold());
    }

    /// <summary>
    /// Recognizes digits in an array of cell images with custom confidence threshold
    /// </summary>
    /// <param name="cellImages">Array of cell images to process</param>
    /// <param name="confidenceThreshold">Minimum confidence threshold</param>
    /// <returns>Array of OCR results</returns>
    public async Task<OCRResult[]> RecognizeDigitsAsync(Image[] cellImages, float confidenceThreshold)
    {
        try
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("OCR service not initialized");
                throw new InvalidOperationException("OCR service is not initialized");
            }

            if (cellImages == null || cellImages.Length != 81)
            {
                _logger.LogWarning("Invalid cell images array: {Length}", cellImages?.Length ?? 0);
                throw new ArgumentException("Must provide exactly 81 cell images");
            }

            _logger.LogInformation("Starting OCR recognition for {CellCount} cells with threshold {Threshold}", 
                cellImages.Length, confidenceThreshold);

            var results = new OCRResult[81];

            for (int i = 0; i < cellImages.Length; i++)
            {
                var row = (i / 9) + 1;
                var col = (i % 9) + 1;
                
                results[i] = await RecognizeSingleCellAsync(cellImages[i], confidenceThreshold, i, row, col);
            }

            var successfulRecognitions = results.Count(r => r.RecognizedDigit > 0);
            var averageConfidence = results.Average(r => r.Confidence);
            
            _logger.LogInformation("OCR recognition completed - {Successful}/{Total} cells recognized, avg confidence: {Confidence}", 
                successfulRecognitions, cellImages.Length, averageConfidence);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during OCR recognition");
            throw;
        }
    }

    /// <summary>
    /// Recognizes a single digit in a cell image
    /// </summary>
    /// <param name="cellImage">The cell image to process</param>
    /// <returns>OCR result for the cell</returns>
    public async Task<OCRResult> RecognizeDigitAsync(Image cellImage)
    {
        return await RecognizeSingleCellAsync(cellImage, GetConfidenceThreshold(), 0, 1, 1);
    }

    /// <summary>
    /// Recognizes a single digit in a cell image with custom parameters
    /// </summary>
    /// <param name="cellImage">The cell image to process</param>
    /// <param name="confidenceThreshold">Minimum confidence threshold</param>
    /// <param name="cellIndex">Index of the cell in the grid</param>
    /// <param name="row">Row number (1-based)</param>
    /// <param name="col">Column number (1-based)</param>
    /// <returns>OCR result for the cell</returns>
    public async Task<OCRResult> RecognizeSingleCellAsync(Image cellImage, float confidenceThreshold, int cellIndex, int row, int col)
    {
        try
        {
            if (cellImage == null)
            {
                _logger.LogWarning("Cell image is null for cell {Index}", cellIndex);
                return OCRResult.CreateEmpty(cellIndex, row, col);
            }

            // Check if cell is empty
            if (OCRUtils.IsCellEmpty(cellImage))
            {
                _logger.LogDebug("Cell {Index} appears to be empty", cellIndex);
                return OCRResult.CreateEmpty(cellIndex, row, col);
            }

            // Preprocess the cell image for better OCR
            var preprocessedImage = await PreprocessForOCRAsync(cellImage);
            
            // In a real implementation, this would use Tesseract OCR
            // For now, we'll simulate OCR recognition
            var (recognizedDigit, confidence, rawText) = await SimulateOCRRecognitionAsync(preprocessedImage);
            
            // Create OCR result
            var result = OCRResult.CreateDigit(recognizedDigit, confidence, rawText, cellIndex, row, col, confidenceThreshold);
            
            _logger.LogDebug("Cell {Index} OCR result - Digit: {Digit}, Confidence: {Confidence}, Raw: '{RawText}'", 
                cellIndex, recognizedDigit, confidence, rawText);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error recognizing digit in cell {Index}", cellIndex);
            return OCRResult.CreateEmpty(cellIndex, row, col);
        }
    }

    /// <summary>
    /// Simulates OCR recognition for testing purposes
    /// </summary>
    /// <param name="cellImage">The cell image to analyze</param>
    /// <returns>Tuple of (digit, confidence, rawText)</returns>
    private async Task<(int digit, float confidence, string rawText)> SimulateOCRRecognitionAsync(Image cellImage)
    {
        // This is a simplified simulation - in a real implementation, you would use Tesseract
        await Task.Delay(10); // Simulate processing time
        
        // Analyze the image to determine if it contains a digit
        var features = OCRUtils.ExtractCellFeatures(cellImage);
        var edgeDensity = (float)features["edgeDensity"];
        var averageBrightness = (float)features["averageBrightness"];
        
        // Simple heuristic: if edge density is high and brightness is low, likely has a digit
        if (edgeDensity > 0.1f && averageBrightness < 0.7f)
        {
            // Simulate recognizing a random digit (1-9)
            var random = new Random();
            var digit = random.Next(1, 10);
            var confidence = 0.7f + (random.NextSingle() * 0.3f); // 0.7-1.0
            
            return (digit, confidence, digit.ToString());
        }
        
        return (0, 0.0f, "");
    }

    private float _confidenceThreshold = 0.7f;

    /// <summary>
    /// Gets the current confidence threshold
    /// </summary>
    /// <returns>Current confidence threshold</returns>
    public float GetConfidenceThreshold()
    {
        return _confidenceThreshold;
    }

    /// <summary>
    /// Sets the confidence threshold for OCR recognition
    /// </summary>
    /// <param name="threshold">New confidence threshold (0.0 to 1.0)</param>
    public void SetConfidenceThreshold(float threshold)
    {
        if (threshold < 0.0f || threshold > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(threshold), "Threshold must be between 0.0 and 1.0");
        
        _confidenceThreshold = threshold;
        _logger.LogDebug("OCR confidence threshold set to {Threshold}", threshold);
    }

    /// <summary>
    /// Checks if the OCR service is initialized
    /// </summary>
    /// <returns>True if initialized, false otherwise</returns>
    public bool IsInitialized()
    {
        return _isInitialized && !_disposed;
    }

    /// <summary>
    /// Gets the confidence score from an OCR result
    /// </summary>
    /// <param name="result">The OCR result</param>
    /// <returns>Confidence score</returns>
    public float GetConfidence(OCRResult result)
    {
        return result?.Confidence ?? 0.0f;
    }

    /// <summary>
    /// Checks if the OCR service is ready to process images
    /// </summary>
    /// <returns>True if ready, false otherwise</returns>
    public async Task<bool> IsReadyAsync()
    {
        try
        {
            // Check if service is initialized and not disposed
            if (!IsInitialized())
            {
                _logger.LogDebug("OCR service not ready - not initialized or disposed");
                return false;
            }

            // In a real implementation, you might check if Tesseract is available
            // For now, we'll assume it's ready if initialized
            await Task.Delay(1); // Simulate async check
            
            _logger.LogDebug("OCR service is ready");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking OCR service readiness");
            return false;
        }
    }

    /// <summary>
    /// Analyzes a cell image to determine its content
    /// </summary>
    /// <param name="cellImage">The cell image to analyze</param>
    /// <returns>OCR result for the cell</returns>
    public async Task<OCRResult> AnalyzeCellAsync(Image cellImage)
    {
        try
        {
            // Check if cell is empty
            if (IsEmptyCell(cellImage))
            {
                return OCRResult.CreateEmpty(0);
            }

            // Extract features for analysis
            var features = OCRUtils.ExtractCellFeatures(cellImage);
            
            // Simple analysis based on features
            var edgeDensity = (float)features["edgeDensity"];
            var averageBrightness = (float)features["averageBrightness"];
            
            if (edgeDensity > 0.15f && averageBrightness < 0.6f)
            {
                // Likely contains a digit
                var confidence = Math.Min(edgeDensity * 2.0f, 1.0f);
                return OCRResult.CreateDigit(1, confidence, "1", 0, 1, 1, 0.5f);
            }
            
            return OCRResult.CreateEmpty(0);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error analyzing cell");
            return OCRResult.CreateEmpty(0);
        }
    }

    /// <summary>
    /// Preprocesses a cell image specifically for OCR
    /// </summary>
    /// <param name="cellImage">The cell image to preprocess</param>
    /// <returns>The preprocessed image</returns>
    private async Task<Image> PreprocessForOCRAsync(Image cellImage)
    {
        try
        {
            // Apply OCR-specific preprocessing
            var preprocessed = cellImage.Clone(ctx => ctx
                .Grayscale()
                .Contrast(1.5f)
                .GaussianBlur(0.5f));

            return await Task.FromResult(preprocessed);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error preprocessing cell for OCR, using original");
            return cellImage;
        }
    }

    /// <summary>
    /// Determines if a cell image is empty based on pixel analysis
    /// </summary>
    /// <param name="cellImage">The cell image to analyze</param>
    /// <returns>True if the cell appears to be empty</returns>
    private bool IsEmptyCell(Image cellImage)
    {
        try
        {
            // Convert to grayscale for analysis
            var grayscaleImage = cellImage.Clone(ctx => ctx.Grayscale());
            
            // Simplified emptiness detection - in a real implementation you would analyze pixel values
            // For now, we'll use a simple heuristic based on image size
            var isEmpty = grayscaleImage.Width < 10 || grayscaleImage.Height < 10;
            
            _logger.LogDebug("Cell emptiness analysis - IsEmpty: {IsEmpty}", isEmpty);
            
            return isEmpty;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error analyzing cell emptiness, assuming not empty");
            return false;
        }
    }

    /// <summary>
    /// Disposes the OCR service and releases resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected dispose method
    /// </summary>
    /// <param name="disposing">True if disposing, false if finalizing</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _isInitialized = false;
            _disposed = true;
            
            _logger.LogInformation("OCR service disposed");
        }
    }
} 