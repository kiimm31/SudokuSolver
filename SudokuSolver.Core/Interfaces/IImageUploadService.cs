using SudokuSolver.Core.Models;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Interfaces;

/// <summary>
/// Defines the contract for the complete image upload and processing pipeline
/// </summary>
public interface IImageUploadService
{
    /// <summary>
    /// Processes an uploaded image to extract Sudoku grid data
    /// </summary>
    /// <param name="imageStream">The uploaded image stream</param>
    /// <param name="options">Processing options</param>
    /// <returns>Complete image upload result with extracted grid and metadata</returns>
    Task<ImageUploadResult> ProcessImageUploadAsync(Stream imageStream, ImageProcessingOptions options);

    /// <summary>
    /// Validates the extracted grid data
    /// </summary>
    /// <param name="result">The image upload result to validate</param>
    /// <returns>True if the result is valid, false otherwise</returns>
    bool ValidateResult(ImageUploadResult result);

    /// <summary>
    /// Handles processing errors and provides meaningful error information
    /// </summary>
    /// <param name="exception">The exception that occurred</param>
    /// <returns>Error result with details about what went wrong</returns>
    ErrorResult HandleErrors(Exception exception);

    /// <summary>
    /// Gets processing statistics and performance metrics
    /// </summary>
    /// <returns>Processing statistics</returns>
    ProcessingStatistics GetProcessingStatistics();

    /// <summary>
    /// Checks if the service is ready to process images
    /// </summary>
    /// <returns>True if ready, false otherwise</returns>
    Task<bool> IsReadyAsync();
} 