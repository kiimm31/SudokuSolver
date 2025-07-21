using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Models;

/// <summary>
/// Complete result of image upload and processing
/// </summary>
public class ImageUploadResult
{
    /// <summary>
    /// Whether the processing was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// The extracted Sudoku grid
    /// </summary>
    public Grid ExtractedGrid { get; set; } = new(new List<Cell>());

    /// <summary>
    /// OCR results for each cell
    /// </summary>
    public OCRResult[] OCRResults { get; set; } = new OCRResult[81];

    /// <summary>
    /// Grid detection result
    /// </summary>
    public GridDetectionResult GridDetection { get; set; } = new();

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Overall confidence score (average of all cell confidences)
    /// </summary>
    public float OverallConfidence { get; set; }

    /// <summary>
    /// Number of cells with low confidence
    /// </summary>
    public int LowConfidenceCells { get; set; }

    /// <summary>
    /// Number of empty cells detected
    /// </summary>
    public int EmptyCells { get; set; }

    /// <summary>
    /// Number of filled cells detected
    /// </summary>
    public int FilledCells { get; set; }

    /// <summary>
    /// Warnings and issues encountered during processing
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Processing statistics
    /// </summary>
    public ProcessingStatistics Statistics { get; set; } = new();

    /// <summary>
    /// Error information if processing failed
    /// </summary>
    public ErrorResult? Error { get; set; }

    /// <summary>
    /// Creates a successful processing result
    /// </summary>
    /// <param name="grid">Extracted Sudoku grid</param>
    /// <param name="ocrResults">OCR results for each cell</param>
    /// <param name="gridDetection">Grid detection result</param>
    /// <param name="processingTimeMs">Processing time</param>
    /// <returns>Successful image upload result</returns>
    public static ImageUploadResult CreateSuccess(Grid grid, OCRResult[] ocrResults, GridDetectionResult gridDetection, long processingTimeMs)
    {
        var overallConfidence = ocrResults.Average(r => r.Confidence);
        var lowConfidenceCells = ocrResults.Count(r => !r.IsConfident);
        var emptyCells = ocrResults.Count(r => r.IsEmpty);
        var filledCells = ocrResults.Count(r => !r.IsEmpty);

        return new ImageUploadResult
        {
            IsSuccess = true,
            ExtractedGrid = grid,
            OCRResults = ocrResults,
            GridDetection = gridDetection,
            ProcessingTimeMs = processingTimeMs,
            OverallConfidence = overallConfidence,
            LowConfidenceCells = lowConfidenceCells,
            EmptyCells = emptyCells,
            FilledCells = filledCells
        };
    }

    /// <summary>
    /// Creates a failed processing result
    /// </summary>
    /// <param name="error">Error information</param>
    /// <param name="processingTimeMs">Processing time</param>
    /// <returns>Failed image upload result</returns>
    public static ImageUploadResult CreateFailure(ErrorResult error, long processingTimeMs)
    {
        return new ImageUploadResult
        {
            IsSuccess = false,
            ProcessingTimeMs = processingTimeMs,
            Error = error
        };
    }
}

/// <summary>
/// Processing statistics and metrics
/// </summary>
public class ProcessingStatistics
{
    /// <summary>
    /// Total number of images processed
    /// </summary>
    public int TotalImagesProcessed { get; set; }

    /// <summary>
    /// Number of successful processing attempts
    /// </summary>
    public int SuccessfulProcessing { get; set; }

    /// <summary>
    /// Number of failed processing attempts
    /// </summary>
    public int FailedProcessing { get; set; }

    /// <summary>
    /// Average processing time in milliseconds
    /// </summary>
    public double AverageProcessingTimeMs { get; set; }

    /// <summary>
    /// Average OCR confidence score
    /// </summary>
    public double AverageOCRConfidence { get; set; }

    /// <summary>
    /// Average grid detection confidence
    /// </summary>
    public double AverageGridDetectionConfidence { get; set; }

    /// <summary>
    /// Memory usage in bytes during processing
    /// </summary>
    public long MemoryUsageBytes { get; set; }

    /// <summary>
    /// Timestamp of last processing
    /// </summary>
    public DateTime LastProcessingTime { get; set; }
}

/// <summary>
/// Error information for failed processing
/// </summary>
public class ErrorResult
{
    /// <summary>
    /// Error type/category
    /// </summary>
    public string ErrorType { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detailed error information
    /// </summary>
    public string Details { get; set; } = string.Empty;

    /// <summary>
    /// Suggested resolution or workaround
    /// </summary>
    public string Suggestion { get; set; } = string.Empty;

    /// <summary>
    /// Error code for programmatic handling
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a validation error
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="details">Error details</param>
    /// <returns>Validation error result</returns>
    public static ErrorResult CreateValidationError(string message, string details = "")
    {
        return new ErrorResult
        {
            ErrorType = "ValidationError",
            Message = message,
            Details = details,
            ErrorCode = "VALIDATION_ERROR"
        };
    }

    /// <summary>
    /// Creates a processing error
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="details">Error details</param>
    /// <returns>Processing error result</returns>
    public static ErrorResult CreateProcessingError(string message, string details = "")
    {
        return new ErrorResult
        {
            ErrorType = "ProcessingError",
            Message = message,
            Details = details,
            ErrorCode = "PROCESSING_ERROR"
        };
    }

    /// <summary>
    /// Creates an OCR error
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="details">Error details</param>
    /// <returns>OCR error result</returns>
    public static ErrorResult CreateOCRError(string message, string details = "")
    {
        return new ErrorResult
        {
            ErrorType = "OCRError",
            Message = message,
            Details = details,
            ErrorCode = "OCR_ERROR"
        };
    }
} 