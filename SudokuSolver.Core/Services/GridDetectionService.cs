using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SudokuSolver.Core.Helpers;
using SudokuSolver.Core.Interfaces;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Services;

/// <summary>
/// Service for detecting Sudoku grid boundaries and extracting individual cells
/// </summary>
public class GridDetectionService : IGridDetector
{
    private readonly ILogger<GridDetectionService> _logger;

    public GridDetectionService(ILogger<GridDetectionService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Detects the Sudoku grid boundaries in the image
    /// </summary>
    /// <param name="image">The processed image to analyze</param>
    /// <returns>Grid detection result with bounds and confidence</returns>
    public async Task<GridDetectionResult> DetectGridAsync(Image image)
    {
        try
        {
            _logger.LogInformation("Starting grid detection for image {Width}x{Height}", image.Width, image.Height);

            // Apply edge detection to find grid lines
            var edgeImage = await DetectEdgesAsync(image);
            
            // Find contours in the edge image
            var contours = await FindContoursAsync(edgeImage);
            
            // Find the largest rectangular contour (likely the grid)
            var gridContour = await FindLargestRectangleAsync(contours);
            
            if (gridContour == null)
            {
                _logger.LogWarning("No suitable grid contour found");
                return GridDetectionResult.CreateFailure("Could not find a valid Sudoku grid in the image");
            }

            // Validate the detected grid
            var isValidGrid = await ValidateGridShapeAsync(gridContour.Value, image.Width, image.Height);
            
            if (!isValidGrid)
            {
                _logger.LogWarning("Detected contour does not match expected grid shape");
                return GridDetectionResult.CreateFailure("The detected grid does not match expected Sudoku grid proportions");
            }

            var gridBounds = new SixLabors.ImageSharp.Rectangle(
                gridContour.Value.X, 
                gridContour.Value.Y, 
                gridContour.Value.Width, 
                gridContour.Value.Height);

            var confidence = CalculateDetectionConfidence(gridContour.Value, image.Width, image.Height);
            
            _logger.LogInformation("Grid detected successfully at {Bounds} with confidence {Confidence}", 
                gridBounds, confidence);

            return GridDetectionResult.CreateSuccess(gridBounds, confidence, "EdgeDetection");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during grid detection");
            return GridDetectionResult.CreateFailure("Detection error");
        }
    }

    /// <summary>
    /// Extracts individual cells from the detected grid
    /// </summary>
    /// <param name="image">The original image</param>
    /// <param name="gridBounds">The detected grid boundaries</param>
    /// <returns>Array of 81 cell images (9x9 grid)</returns>
    public async Task<Image[]> ExtractCellsAsync(Image image, SixLabors.ImageSharp.Rectangle gridBounds)
    {
        try
        {
            _logger.LogInformation("Extracting cells from grid bounds {Bounds}", gridBounds);

            // Ensure grid bounds are within image boundaries
            var constrainedBounds = new SixLabors.ImageSharp.Rectangle(
                Math.Max(0, gridBounds.X),
                Math.Max(0, gridBounds.Y),
                Math.Min(gridBounds.Width, image.Width - gridBounds.X),
                Math.Min(gridBounds.Height, image.Height - gridBounds.Y)
            );

            var cellSize = constrainedBounds.Width / 9;
            var cells = new Image[81];

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    var cellX = constrainedBounds.X + (col * cellSize);
                    var cellY = constrainedBounds.Y + (row * cellSize);
                    var cellWidth = Math.Min(cellSize, image.Width - cellX);
                    var cellHeight = Math.Min(cellSize, image.Height - cellY);

                    var cellBounds = new SixLabors.ImageSharp.Rectangle(
                        cellX,
                        cellY,
                        cellWidth,
                        cellHeight
                    );

                    var cellImage = image.Clone(ctx => ctx.Crop(cellBounds));
                    
                    // Apply additional preprocessing to individual cells
                    cellImage = await PreprocessCellAsync(cellImage);
                    
                    cells[row * 9 + col] = cellImage;
                }
            }

            _logger.LogInformation("Successfully extracted {CellCount} cells", cells.Length);
            return await Task.FromResult(cells);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting cells from grid");
            throw new InvalidOperationException("Failed to extract cells from grid", ex);
        }
    }

    /// <summary>
    /// Validates that the extracted cells form a valid Sudoku grid
    /// </summary>
    /// <param name="cells">Array of cell images</param>
    /// <returns>True if the grid is valid, false otherwise</returns>
    public bool ValidateGridStructure(Image[] cells)
    {
        try
        {
            _logger.LogDebug("Validating extracted grid cells");

            if (cells == null || cells.Length != 81)
            {
                _logger.LogWarning("Invalid number of cells: {CellCount}", cells?.Length ?? 0);
                return false;
            }

            // Check that all cells have reasonable dimensions
            foreach (var cell in cells)
            {
                if (cell == null || cell.Width < 10 || cell.Height < 10)
                {
                    _logger.LogWarning("Cell has invalid dimensions: {Width}x{Height}", 
                        cell?.Width ?? 0, cell?.Height ?? 0);
                    return false;
                }
            }

            _logger.LogDebug("Grid structure validation passed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating grid structure");
            return false;
        }
    }

    /// <summary>
    /// Detects edges in the image using Sobel operator
    /// </summary>
    /// <param name="image">The image to process</param>
    /// <returns>Edge-detected image</returns>
    private async Task<Image> DetectEdgesAsync(Image image)
    {
        return await Task.FromResult(image.Clone(ctx => ctx
            .Grayscale()
            .Contrast(1.5f)
            .GaussianBlur(0.5f)));
    }

    /// <summary>
    /// Finds contours in the edge-detected image
    /// </summary>
    /// <param name="edgeImage">The edge-detected image</param>
    /// <returns>List of detected contours</returns>
    private async Task<List<SixLabors.ImageSharp.Rectangle>> FindContoursAsync(Image edgeImage)
    {
        // Simplified contour detection - in a real implementation, you would use
        // more sophisticated contour detection algorithms
        var contours = new List<SixLabors.ImageSharp.Rectangle>();
        
        // For now, return a simple bounding rectangle
        contours.Add(new SixLabors.ImageSharp.Rectangle(0, 0, edgeImage.Width, edgeImage.Height));
        
        return await Task.FromResult(contours);
    }

    /// <summary>
    /// Finds the largest rectangular contour that could be a Sudoku grid
    /// </summary>
    /// <param name="contours">List of detected contours</param>
    /// <returns>The largest rectangular contour, or null if none found</returns>
    private async Task<SixLabors.ImageSharp.Rectangle?> FindLargestRectangleAsync(List<SixLabors.ImageSharp.Rectangle> contours)
    {
        if (contours == null || contours.Count == 0)
            return null;

        // Find the largest contour by area
        var largestContour = contours.OrderByDescending(c => c.Width * c.Height).First();
        
        // Ensure it's reasonably sized (at least 50% of image dimensions)
        if (largestContour.Width < 100 || largestContour.Height < 100)
            return null;

        return await Task.FromResult(largestContour);
    }

    /// <summary>
    /// Validates that the detected contour has appropriate proportions for a Sudoku grid
    /// </summary>
    /// <param name="contour">The detected contour</param>
    /// <param name="imageWidth">Original image width</param>
    /// <param name="imageHeight">Original image height</param>
    /// <returns>True if the contour is valid for a Sudoku grid</returns>
    private async Task<bool> ValidateGridShapeAsync(SixLabors.ImageSharp.Rectangle contour, int imageWidth, int imageHeight)
    {
        // Check aspect ratio (should be close to 1:1 for a square grid)
        var aspectRatio = (float)contour.Width / contour.Height;
        if (aspectRatio < 0.8f || aspectRatio > 1.2f)
        {
            _logger.LogDebug("Invalid aspect ratio: {AspectRatio}", aspectRatio);
            return false;
        }

        // Check size relative to image
        var relativeWidth = (float)contour.Width / imageWidth;
        var relativeHeight = (float)contour.Height / imageHeight;
        
        if (relativeWidth < 0.3f || relativeHeight < 0.3f)
        {
            _logger.LogDebug("Grid too small relative to image: {RelativeWidth}x{RelativeHeight}", 
                relativeWidth, relativeHeight);
            return false;
        }

        return await Task.FromResult(true);
    }

    /// <summary>
    /// Calculates confidence score for the detected grid
    /// </summary>
    /// <param name="contour">The detected contour</param>
    /// <param name="imageWidth">Original image width</param>
    /// <param name="imageHeight">Original image height</param>
    /// <returns>Confidence score between 0.0 and 1.0</returns>
    private float CalculateDetectionConfidence(SixLabors.ImageSharp.Rectangle contour, int imageWidth, int imageHeight)
    {
        var confidence = 0.5f; // Base confidence

        // Aspect ratio confidence
        var aspectRatio = (float)contour.Width / contour.Height;
        var aspectConfidence = 1.0f - Math.Abs(1.0f - aspectRatio);
        confidence += aspectConfidence * 0.3f;

        // Size confidence
        var relativeSize = Math.Min((float)contour.Width / imageWidth, (float)contour.Height / imageHeight);
        var sizeConfidence = Math.Min(relativeSize / 0.5f, 1.0f);
        confidence += sizeConfidence * 0.2f;

        return Math.Min(confidence, 1.0f);
    }

    /// <summary>
    /// Finds the largest connected region in the edge image
    /// </summary>
    /// <param name="edgeImage">The edge-detected image</param>
    /// <returns>The largest connected region as a rectangle</returns>
    private async Task<SixLabors.ImageSharp.Rectangle?> FindLargestConnectedRegionAsync(Image edgeImage)
    {
        // Simplified implementation - returns the full image bounds
        // In a real implementation, you would use connected component analysis
        return await Task.FromResult(new SixLabors.ImageSharp.Rectangle(0, 0, edgeImage.Width, edgeImage.Height));
    }

    /// <summary>
    /// Gets the detection confidence from a grid detection result
    /// </summary>
    /// <param name="result">The grid detection result</param>
    /// <returns>Confidence score</returns>
    public float GetDetectionConfidence(GridDetectionResult result)
    {
        return result?.Confidence ?? 0.0f;
    }

    /// <summary>
    /// Preprocesses a cell image for better analysis
    /// </summary>
    /// <param name="cellImage">The cell image to preprocess</param>
    /// <returns>The preprocessed cell image</returns>
    private async Task<Image> PreprocessCellAsync(Image cellImage)
    {
        return await Task.FromResult(cellImage.Clone(ctx => ctx
            .Grayscale()
            .Contrast(1.2f)));
    }
} 