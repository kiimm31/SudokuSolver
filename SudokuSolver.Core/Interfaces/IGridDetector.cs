using SixLabors.ImageSharp;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Interfaces;

/// <summary>
/// Defines the contract for Sudoku grid detection operations
/// </summary>
public interface IGridDetector
{
    /// <summary>
    /// Detects the Sudoku grid boundaries in an image
    /// </summary>
    /// <param name="image">The processed image</param>
    /// <returns>Grid detection result with boundaries and confidence</returns>
    Task<GridDetectionResult> DetectGridAsync(Image image);

    /// <summary>
    /// Extracts individual cells from the detected grid
    /// </summary>
    /// <param name="image">The processed image</param>
    /// <param name="gridBounds">The detected grid boundaries</param>
    /// <returns>Array of 81 cell images (9x9 grid)</returns>
    Task<Image[]> ExtractCellsAsync(Image image, Rectangle gridBounds);

    /// <summary>
    /// Validates if the detected grid structure is valid
    /// </summary>
    /// <param name="cells">The extracted cell images</param>
    /// <returns>True if the grid structure is valid, false otherwise</returns>
    bool ValidateGridStructure(Image[] cells);

    /// <summary>
    /// Gets the confidence score for grid detection
    /// </summary>
    /// <param name="result">The grid detection result</param>
    /// <returns>Confidence score between 0 and 1</returns>
    float GetDetectionConfidence(GridDetectionResult result);
} 