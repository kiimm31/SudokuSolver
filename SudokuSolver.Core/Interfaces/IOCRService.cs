using SixLabors.ImageSharp;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Interfaces;

/// <summary>
/// Defines the contract for OCR (Optical Character Recognition) operations
/// </summary>
public interface IOCRService : IDisposable
{
    /// <summary>
    /// Initializes the OCR engine
    /// </summary>
    /// <param name="tessdataPath">Path to Tesseract training data</param>
    /// <returns>True if initialization was successful, false otherwise</returns>
    Task<bool> InitializeAsync(string tessdataPath);

    /// <summary>
    /// Recognizes digits in individual cell images
    /// </summary>
    /// <param name="cellImages">Array of cell images to process</param>
    /// <returns>Array of OCR results for each cell</returns>
    Task<OCRResult[]> RecognizeDigitsAsync(Image[] cellImages);

    /// <summary>
    /// Recognizes digits in individual cell images with custom confidence threshold
    /// </summary>
    /// <param name="cellImages">Array of cell images to process</param>
    /// <param name="confidenceThreshold">Minimum confidence threshold</param>
    /// <returns>Array of OCR results for each cell</returns>
    Task<OCRResult[]> RecognizeDigitsAsync(Image[] cellImages, float confidenceThreshold);

    /// <summary>
    /// Recognizes a single digit in a cell image
    /// </summary>
    /// <param name="cellImage">The cell image to process</param>
    /// <returns>OCR result for the cell</returns>
    Task<OCRResult> RecognizeDigitAsync(Image cellImage);

    /// <summary>
    /// Recognizes a single digit in a cell image with custom parameters
    /// </summary>
    /// <param name="cellImage">The cell image to process</param>
    /// <param name="confidenceThreshold">Minimum confidence threshold</param>
    /// <param name="cellIndex">Index of the cell in the grid</param>
    /// <param name="row">Row number (1-based)</param>
    /// <param name="col">Column number (1-based)</param>
    /// <returns>OCR result for the cell</returns>
    Task<OCRResult> RecognizeSingleCellAsync(Image cellImage, float confidenceThreshold, int cellIndex, int row, int col);

    /// <summary>
    /// Gets the confidence threshold for accepting OCR results
    /// </summary>
    /// <returns>Confidence threshold (0.0 to 1.0)</returns>
    float GetConfidenceThreshold();

    /// <summary>
    /// Sets the confidence threshold for accepting OCR results
    /// </summary>
    /// <param name="threshold">Confidence threshold (0.0 to 1.0)</param>
    void SetConfidenceThreshold(float threshold);

    /// <summary>
    /// Checks if the OCR engine is properly initialized
    /// </summary>
    /// <returns>True if initialized, false otherwise</returns>
    bool IsInitialized();

    /// <summary>
    /// Gets the confidence score for an OCR result
    /// </summary>
    /// <param name="result">The OCR result</param>
    /// <returns>Confidence score between 0 and 1</returns>
    float GetConfidence(OCRResult result);

    /// <summary>
    /// Checks if the OCR engine is ready to process images
    /// </summary>
    /// <returns>True if ready, false otherwise</returns>
    Task<bool> IsReadyAsync();

    /// <summary>
    /// Analyzes a cell image to determine its content
    /// </summary>
    /// <param name="cellImage">The cell image to analyze</param>
    /// <returns>OCR result for the cell</returns>
    Task<OCRResult> AnalyzeCellAsync(Image cellImage);
} 