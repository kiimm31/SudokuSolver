using SixLabors.ImageSharp;

namespace SudokuSolver.Core.Models;

/// <summary>
/// Result of Sudoku grid detection
/// </summary>
public class GridDetectionResult
{
    /// <summary>
    /// Whether a grid was successfully detected
    /// </summary>
    public bool IsDetected { get; set; }

    /// <summary>
    /// The detected grid boundaries
    /// </summary>
    public Rectangle GridBounds { get; set; }

    /// <summary>
    /// Confidence score for the detection (0.0 to 1.0)
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// The detected grid lines (horizontal and vertical)
    /// </summary>
    public List<Line> GridLines { get; set; } = new();

    /// <summary>
    /// Individual cell boundaries (9x9 = 81 cells)
    /// </summary>
    public Rectangle[] CellBounds { get; set; } = new Rectangle[81];

    /// <summary>
    /// Detection method used
    /// </summary>
    public string DetectionMethod { get; set; } = string.Empty;

    /// <summary>
    /// Additional detection metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Creates a successful detection result
    /// </summary>
    /// <param name="gridBounds">Detected grid boundaries</param>
    /// <param name="confidence">Detection confidence</param>
    /// <param name="detectionMethod">Method used for detection</param>
    /// <returns>Successful grid detection result</returns>
    public static GridDetectionResult CreateSuccess(Rectangle gridBounds, float confidence, string detectionMethod)
    {
        return new GridDetectionResult
        {
            IsDetected = true,
            GridBounds = gridBounds,
            Confidence = confidence,
            DetectionMethod = detectionMethod
        };
    }

    /// <summary>
    /// Creates a failed detection result
    /// </summary>
    /// <param name="reason">Reason for failure</param>
    /// <returns>Failed grid detection result</returns>
    public static GridDetectionResult CreateFailure(string reason)
    {
        return new GridDetectionResult
        {
            IsDetected = false,
            Confidence = 0.0f,
            DetectionMethod = "None",
            Metadata = { ["failureReason"] = reason }
        };
    }
}

/// <summary>
/// Represents a line in the grid
/// </summary>
public class Line
{
    /// <summary>
    /// Start point of the line
    /// </summary>
    public Point Start { get; set; }

    /// <summary>
    /// End point of the line
    /// </summary>
    public Point End { get; set; }

    /// <summary>
    /// Whether this is a horizontal line
    /// </summary>
    public bool IsHorizontal { get; set; }

    /// <summary>
    /// Line thickness
    /// </summary>
    public float Thickness { get; set; }

    /// <summary>
    /// Line confidence score
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// Creates a horizontal line
    /// </summary>
    /// <param name="y">Y coordinate</param>
    /// <param name="startX">Start X coordinate</param>
    /// <param name="endX">End X coordinate</param>
    /// <param name="thickness">Line thickness</param>
    /// <param name="confidence">Line confidence</param>
    /// <returns>Horizontal line</returns>
    public static Line CreateHorizontal(int y, int startX, int endX, float thickness, float confidence)
    {
        return new Line
        {
            Start = new Point(startX, y),
            End = new Point(endX, y),
            IsHorizontal = true,
            Thickness = thickness,
            Confidence = confidence
        };
    }

    /// <summary>
    /// Creates a vertical line
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="startY">Start Y coordinate</param>
    /// <param name="endY">End Y coordinate</param>
    /// <param name="thickness">Line thickness</param>
    /// <param name="confidence">Line confidence</param>
    /// <returns>Vertical line</returns>
    public static Line CreateVertical(int x, int startY, int endY, float thickness, float confidence)
    {
        return new Line
        {
            Start = new Point(x, startY),
            End = new Point(x, endY),
            IsHorizontal = false,
            Thickness = thickness,
            Confidence = confidence
        };
    }
} 