using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SudokuSolver.Core.Helpers;
using SudokuSolver.Core.Interfaces;
using SudokuSolver.Core.Models;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Services;

/// <summary>
/// Service for processing image uploads and extracting Sudoku puzzles
/// </summary>
public class ImageUploadService : IImageUploadService
{
    private readonly ILogger<ImageUploadService> _logger;
    private readonly IImageProcessor _imageProcessor;
    private readonly IGridDetector _gridDetector;
    private readonly IOCRService _ocrService;
    private readonly ProcessingStatistics _statistics;

    public ImageUploadService(
        ILogger<ImageUploadService> logger,
        IImageProcessor imageProcessor,
        IGridDetector gridDetector,
        IOCRService ocrService)
    {
        _logger = logger;
        _imageProcessor = imageProcessor;
        _gridDetector = gridDetector;
        _ocrService = ocrService;
        _statistics = new ProcessingStatistics();
    }

    /// <summary>
    /// Processes an uploaded image to extract a Sudoku puzzle
    /// </summary>
    /// <param name="imageStream">The image stream to process</param>
    /// <param name="options">Processing options</param>
    /// <returns>Result containing the extracted Sudoku grid</returns>
    public async Task<ImageUploadResult> ProcessImageUploadAsync(Stream imageStream, ImageProcessingOptions options)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation("Starting image upload processing");

            // Step 1: Validate input
            _logger.LogDebug("Step 1: Validating input stream");
            if (imageStream == null || !imageStream.CanRead)
            {
                var error = new ErrorResult
                {
                    ErrorType = "ValidationError",
                    Message = "Invalid image stream",
                    Details = "The provided image stream is null or cannot be read.",
                    Suggestion = "Please provide a valid image file.",
                    ErrorCode = "INVALID_STREAM"
                };
                
                return ImageUploadResult.CreateFailure(error, stopwatch.ElapsedMilliseconds);
            }

            // Step 2: Validate file format and size
            _logger.LogDebug("Step 2: Validating file format");
            // Create a fresh stream for format validation
            var formatValidationStream = CreateFreshStream(imageStream);
            if (!await ImageUtils.IsSupportedFormatAsync(formatValidationStream))
            {
                var error = new ErrorResult
                {
                    ErrorType = "ValidationError",
                    Message = "Unsupported image format",
                    Details = "The uploaded file is not a supported image format.",
                    Suggestion = "Please upload a JPG, PNG, WebP, or BMP image.",
                    ErrorCode = "UNSUPPORTED_FORMAT"
                };
                
                return ImageUploadResult.CreateFailure(error, stopwatch.ElapsedMilliseconds);
            }

            _logger.LogDebug("Step 2b: Validating file size");
            // Create a fresh stream for size validation
            var sizeValidationStream = CreateFreshStream(imageStream);
            if (!ImageUtils.IsValidFileSize(sizeValidationStream))
            {
                var error = new ErrorResult
                {
                    ErrorType = "ValidationError",
                    Message = "File too large",
                    Details = $"The uploaded file exceeds the maximum size of {ImageUtils.MaxFileSizeBytes / (1024 * 1024)}MB.",
                    Suggestion = "Please upload a smaller image file.",
                    ErrorCode = "FILE_TOO_LARGE"
                };
                
                return ImageUploadResult.CreateFailure(error, stopwatch.ElapsedMilliseconds);
            }

            // Step 3: Process the image
            _logger.LogDebug("Step 3: Processing the image");
            // Create a fresh stream for image processing
            var processingStream = CreateFreshStream(imageStream);
            var processedImage = await _imageProcessor.PreprocessImageAsync(processingStream, options);
            _logger.LogDebug("Image preprocessing completed");

            // Step 4: Detect Sudoku grid
            _logger.LogDebug("Step 4: Detecting Sudoku grid");
            var gridDetectionResult = await _gridDetector.DetectGridAsync(processedImage);
            
            if (!gridDetectionResult.IsDetected)
            {
                var error = new ErrorResult
                {
                    ErrorType = "DetectionError",
                    Message = "Could not detect Sudoku grid",
                    Details = "No valid Sudoku grid was found in the image.",
                    Suggestion = "Please ensure the image contains a clear, well-defined Sudoku puzzle.",
                    ErrorCode = "GRID_NOT_DETECTED"
                };
                
                return ImageUploadResult.CreateFailure(error, stopwatch.ElapsedMilliseconds);
            }

            _logger.LogDebug("Grid detection completed with confidence {Confidence}", gridDetectionResult.Confidence);

            // Step 5: Extract individual cells
            _logger.LogDebug("Step 5: Extracting individual cells");
            var cellImages = await _gridDetector.ExtractCellsAsync(processedImage, gridDetectionResult.GridBounds);
            
            if (!_gridDetector.ValidateGridStructure(cellImages))
            {
                var error = new ErrorResult
                {
                    ErrorType = "ValidationError",
                    Message = "Invalid grid structure",
                    Details = "The detected grid does not have the expected structure.",
                    Suggestion = "Please ensure the Sudoku grid is clearly visible and properly aligned.",
                    ErrorCode = "INVALID_GRID_STRUCTURE"
                };
                
                return ImageUploadResult.CreateFailure(error, stopwatch.ElapsedMilliseconds);
            }

            // Step 6: Perform OCR on each cell
            _logger.LogDebug("Step 6: Performing OCR on cells");
            var ocrResults = await _ocrService.RecognizeDigitsAsync(cellImages, options.ConfidenceThreshold);
            _logger.LogDebug("OCR processing completed");

            // Step 7: Convert OCR results to Sudoku grid
            _logger.LogDebug("Step 7: Converting OCR results to grid");
            var extractedGrid = ConvertOCRResultsToGrid(ocrResults);
            
            // Step 8: Validate the extracted grid
            _logger.LogDebug("Step 8: Validating extracted grid");
            var validationResult = extractedGrid.ObeysAllConstraint();
            
            if (!validationResult)
            {
                var error = new ErrorResult
                {
                    ErrorType = "ValidationError",
                    Message = "Invalid Sudoku puzzle",
                    Details = "The extracted numbers do not form a valid Sudoku puzzle.",
                    Suggestion = "Check that the extracted numbers form a valid Sudoku puzzle.",
                    ErrorCode = "INVALID_SUDOKU_PUZZLE"
                };
                
                return ImageUploadResult.CreateFailure(error, stopwatch.ElapsedMilliseconds);
            }

            // Step 9: Create success result
            stopwatch.Stop();
            var result = ImageUploadResult.CreateSuccess(extractedGrid, ocrResults, gridDetectionResult, stopwatch.ElapsedMilliseconds);
            
            // Update statistics
            _statistics.SuccessfulProcessing++;
            _statistics.AverageProcessingTimeMs = CalculateAverageProcessingTime();
            _statistics.AverageOCRConfidence = ocrResults.Average(r => r.Confidence);
            _statistics.AverageGridDetectionConfidence = gridDetectionResult.Confidence;
            _statistics.MemoryUsageBytes = GC.GetTotalMemory(false);

            _logger.LogInformation("Image upload processing completed successfully in {ProcessingTime}ms", 
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error during image upload processing: {Message}", ex.Message);
            
            _statistics.FailedProcessing++;
            
            var error = HandleErrors(ex);
            return ImageUploadResult.CreateFailure(error, stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Validates the extracted grid data
    /// </summary>
    /// <param name="result">The image upload result to validate</param>
    /// <returns>True if the result is valid, false otherwise</returns>
    public bool ValidateResult(ImageUploadResult result)
    {
        try
        {
            if (result == null)
                return false;

            if (!result.IsSuccess)
                return false;

            if (result.ExtractedGrid == null)
                return false;

            // Check that we have the expected number of cells
            var allCells = result.ExtractedGrid.GetAllCells();
            if (allCells == null || allCells.Count != 81)
                return false;

            // Check that OCR results match the grid
            if (result.OCRResults == null || result.OCRResults.Length != 81)
                return false;

            // Validate grid structure
            var validationResult = result.ExtractedGrid.ObeysAllConstraint();
            
            return validationResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating result");
            return false;
        }
    }

    /// <summary>
    /// Handles different types of exceptions and converts them to appropriate error results
    /// </summary>
    /// <param name="exception">The exception to handle</param>
    /// <returns>Error result with appropriate details</returns>
    public ErrorResult HandleErrors(Exception exception)
    {
        _logger.LogError(exception, "Handling error during image processing");

        switch (exception)
        {
            case ArgumentException:
                return new ErrorResult
                {
                    ErrorType = "ValidationError",
                    Message = "Invalid input parameters",
                    Details = exception.Message,
                    Suggestion = "Please check the input parameters and try again.",
                    ErrorCode = "INVALID_PARAMETERS"
                };

            case InvalidOperationException:
                return new ErrorResult
                {
                    ErrorType = "ProcessingError",
                    Message = "Processing operation failed",
                    Details = exception.Message,
                    Suggestion = "Please try again with a different image.",
                    ErrorCode = "PROCESSING_FAILED"
                };

            case OutOfMemoryException:
                return new ErrorResult
                {
                    ErrorType = "SystemError",
                    Message = "Insufficient memory",
                    Details = "The system ran out of memory while processing the image.",
                    Suggestion = "Please try with a smaller image or try again later.",
                    ErrorCode = "OUT_OF_MEMORY"
                };

            case TimeoutException:
                return new ErrorResult
                {
                    ErrorType = "TimeoutError",
                    Message = "Processing timeout",
                    Details = "The image processing operation took too long to complete.",
                    Suggestion = "Please try with a smaller or simpler image.",
                    ErrorCode = "PROCESSING_TIMEOUT"
                };

            default:
                return new ErrorResult
                {
                    ErrorType = "UnknownError",
                    Message = "An unknown error occurred",
                    Details = exception.Message,
                    Suggestion = "Please try again or contact support.",
                    ErrorCode = "UNKNOWN_ERROR"
                };
        }
    }

    /// <summary>
    /// Gets processing statistics and performance metrics
    /// </summary>
    /// <returns>Processing statistics</returns>
    public ProcessingStatistics GetProcessingStatistics()
    {
        return _statistics;
    }

    /// <summary>
    /// Checks if the service is ready to process images
    /// </summary>
    /// <returns>True if ready, false otherwise</returns>
    public async Task<bool> IsReadyAsync()
    {
        try
        {
            // Check if OCR service is ready
            var ocrReady = await _ocrService.IsReadyAsync();
            
            if (!ocrReady)
            {
                _logger.LogWarning("OCR service is not ready");
                return false;
            }

            _logger.LogDebug("Image upload service is ready");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking service readiness");
            return false;
        }
    }

    /// <summary>
    /// Converts OCR results to a Sudoku grid
    /// </summary>
    /// <param name="ocrResults">Array of OCR results for each cell</param>
    /// <returns>Sudoku grid with extracted values</returns>
    private Grid ConvertOCRResultsToGrid(OCRResult[] ocrResults)
    {
        try
        {
            var cells = new List<Cell>();

            for (int i = 0; i < ocrResults.Length; i++)
            {
                var row = (i / 9) + 1; // Convert to 1-based indexing
                var col = (i % 9) + 1; // Convert to 1-based indexing
                var ocrResult = ocrResults[i];

                var cell = new Cell
                {
                    Row = row,
                    Column = col
                };

                if (!ocrResult.IsEmpty)
                {
                    cell.SetValue(ocrResult.RecognizedDigit);
                }

                cells.Add(cell);
            }

            var grid = new Grid(cells);
            _logger.LogDebug("Converted OCR results to grid with {FilledCells} filled cells", 
                cells.Count(c => c.Value > 0));

            return grid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting OCR results to grid");
            throw new InvalidOperationException("Failed to convert OCR results to grid", ex);
        }
    }

    /// <summary>
    /// Calculates the average processing time
    /// </summary>
    /// <returns>Average processing time in milliseconds</returns>
    private double CalculateAverageProcessingTime()
    {
        if (_statistics.SuccessfulProcessing == 0)
            return 0.0;

        // This is a simplified calculation - in a real implementation,
        // you might want to maintain a rolling average or use a more sophisticated approach
        return _statistics.AverageProcessingTimeMs;
    }

    /// <summary>
    /// Creates a fresh stream from the original stream
    /// </summary>
    /// <param name="originalStream">The original stream</param>
    /// <returns>A fresh stream with the same content</returns>
    private Stream CreateFreshStream(Stream originalStream)
    {
        var memoryStream = new MemoryStream();
        originalStream.Position = 0;
        originalStream.CopyTo(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }
} 