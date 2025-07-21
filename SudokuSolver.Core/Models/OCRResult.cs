namespace SudokuSolver.Core.Models;

/// <summary>
/// Result of OCR processing for a single cell
/// </summary>
public class OCRResult
{
    /// <summary>
    /// The recognized digit (0-9, where 0 represents empty cell)
    /// </summary>
    public int RecognizedDigit { get; set; }

    /// <summary>
    /// Confidence score for the recognition (0.0 to 1.0)
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// The raw text recognized by OCR
    /// </summary>
    public string RawText { get; set; } = string.Empty;

    /// <summary>
    /// Whether the cell is considered empty
    /// </summary>
    public bool IsEmpty { get; set; }

    /// <summary>
    /// Whether the confidence meets the threshold for acceptance
    /// </summary>
    public bool IsConfident { get; set; }

    /// <summary>
    /// Cell position in the grid (0-80 for 9x9 grid)
    /// </summary>
    public int CellIndex { get; set; }

    /// <summary>
    /// Row position in the grid (0-8)
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Column position in the grid (0-8)
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Additional OCR metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Creates an empty cell result
    /// </summary>
    /// <param name="cellIndex">Cell index</param>
    /// <param name="row">Row position</param>
    /// <param name="column">Column position</param>
    /// <returns>OCR result for an empty cell</returns>
    public static OCRResult CreateEmpty(int cellIndex, int row = 0, int column = 0)
    {
        return new OCRResult
        {
            RecognizedDigit = 0,
            Confidence = 1.0f,
            RawText = string.Empty,
            IsEmpty = true,
            IsConfident = true,
            CellIndex = cellIndex,
            Row = row,
            Column = column
        };
    }

    /// <summary>
    /// Creates a digit recognition result
    /// </summary>
    /// <param name="digit">Recognized digit (1-9)</param>
    /// <param name="confidence">Confidence score</param>
    /// <param name="rawText">Raw OCR text</param>
    /// <param name="cellIndex">Cell index</param>
    /// <param name="row">Row position</param>
    /// <param name="column">Column position</param>
    /// <param name="confidenceThreshold">Confidence threshold</param>
    /// <returns>OCR result for a digit</returns>
    public static OCRResult CreateDigit(int digit, float confidence, string rawText, int cellIndex, int row, int column, float confidenceThreshold)
    {
        return new OCRResult
        {
            RecognizedDigit = digit,
            Confidence = confidence,
            RawText = rawText,
            IsEmpty = false,
            IsConfident = confidence >= confidenceThreshold,
            CellIndex = cellIndex,
            Row = row,
            Column = column
        };
    }

    /// <summary>
    /// Creates a low-confidence result
    /// </summary>
    /// <param name="rawText">Raw OCR text</param>
    /// <param name="confidence">Confidence score</param>
    /// <param name="cellIndex">Cell index</param>
    /// <param name="row">Row position</param>
    /// <param name="column">Column position</param>
    /// <returns>OCR result for low-confidence recognition</returns>
    public static OCRResult CreateLowConfidence(string rawText, float confidence, int cellIndex, int row = 0, int column = 0)
    {
        return new OCRResult
        {
            RecognizedDigit = 0,
            Confidence = confidence,
            RawText = rawText,
            IsEmpty = false,
            IsConfident = false,
            CellIndex = cellIndex,
            Row = row,
            Column = column
        };
    }

    /// <summary>
    /// Creates a successful digit recognition result
    /// </summary>
    /// <param name="digit">Recognized digit (1-9)</param>
    /// <param name="confidence">Confidence score</param>
    /// <param name="rawText">Raw OCR text</param>
    /// <param name="isConfident">Whether the result meets confidence threshold</param>
    /// <param name="cellIndex">Cell index</param>
    /// <param name="row">Row position</param>
    /// <param name="column">Column position</param>
    /// <returns>OCR result for successful digit recognition</returns>
    public static OCRResult CreateSuccess(int digit, float confidence, string rawText, bool isConfident, int cellIndex, int row = 0, int column = 0)
    {
        return new OCRResult
        {
            RecognizedDigit = digit,
            Confidence = confidence,
            RawText = rawText,
            IsEmpty = false,
            IsConfident = isConfident,
            CellIndex = cellIndex,
            Row = row,
            Column = column
        };
    }
} 