using System.Text.Json.Serialization;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Api.Models;

/// <summary>
/// Response model for Sudoku solve operations
/// </summary>
public class SudokuSolveResponse
{
    /// <summary>
    /// The original unsolved grid
    /// </summary>
    [JsonPropertyName("originalGrid")]
    public Grid OriginalGrid { get; set; } = new(new List<Cell>());

    /// <summary>
    /// The solved grid (null if unsolvable)
    /// </summary>
    [JsonPropertyName("solvedGrid")]
    public Grid? SolvedGrid { get; set; }

    /// <summary>
    /// Whether the puzzle was successfully solved
    /// </summary>
    [JsonPropertyName("isSolvable")]
    public bool IsSolvable { get; set; }

    /// <summary>
    /// Time taken to solve in milliseconds
    /// </summary>
    [JsonPropertyName("solveTimeMs")]
    public long SolveTimeMs { get; set; }

    /// <summary>
    /// Number of solving steps taken
    /// </summary>
    [JsonPropertyName("stepsTaken")]
    public int StepsTaken { get; set; }

    /// <summary>
    /// Solving strategies used
    /// </summary>
    [JsonPropertyName("strategiesUsed")]
    public List<string> StrategiesUsed { get; set; } = new();

    /// <summary>
    /// Difficulty assessment of the puzzle
    /// </summary>
    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = "Unknown";

    /// <summary>
    /// Additional solving metadata
    /// </summary>
    [JsonPropertyName("metadata")]
    public SolvingMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Response model for Sudoku validation operations
/// </summary>
public class SudokuValidationResponse
{
    /// <summary>
    /// Whether the grid is valid
    /// </summary>
    [JsonPropertyName("isValid")]
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation errors found
    /// </summary>
    [JsonPropertyName("errors")]
    public List<ValidationError> Errors { get; set; } = new();

    /// <summary>
    /// Whether the grid is complete (all cells filled)
    /// </summary>
    [JsonPropertyName("isComplete")]
    public bool IsComplete { get; set; }

    /// <summary>
    /// Number of filled cells
    /// </summary>
    [JsonPropertyName("filledCells")]
    public int FilledCells { get; set; }

    /// <summary>
    /// Number of empty cells
    /// </summary>
    [JsonPropertyName("emptyCells")]
    public int EmptyCells { get; set; }
}

/// <summary>
/// Response model for Sudoku generation operations
/// </summary>
public class SudokuGenerationResponse
{
    /// <summary>
    /// The generated puzzle grid
    /// </summary>
    [JsonPropertyName("puzzle")]
    public Grid Puzzle { get; set; } = new(new List<Cell>());

    /// <summary>
    /// The complete solution grid
    /// </summary>
    [JsonPropertyName("solution")]
    public Grid Solution { get; set; } = new(new List<Cell>());

    /// <summary>
    /// The requested difficulty level
    /// </summary>
    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = string.Empty;

    /// <summary>
    /// Time taken to generate in milliseconds
    /// </summary>
    [JsonPropertyName("generationTimeMs")]
    public long GenerationTimeMs { get; set; }

    /// <summary>
    /// Number of clues in the puzzle
    /// </summary>
    [JsonPropertyName("clueCount")]
    public int ClueCount { get; set; }

    /// <summary>
    /// Generation metadata
    /// </summary>
    [JsonPropertyName("metadata")]
    public GenerationMetadata Metadata { get; set; } = new();
}

/// <summary>
/// Validation error details
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Error type/category
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Cell position where error occurred (if applicable)
    /// </summary>
    [JsonPropertyName("position")]
    public CellPosition? Position { get; set; }
}

/// <summary>
/// Cell position information
/// </summary>
public class CellPosition
{
    /// <summary>
    /// Row index (0-based)
    /// </summary>
    [JsonPropertyName("row")]
    public int Row { get; set; }

    /// <summary>
    /// Column index (0-based)
    /// </summary>
    [JsonPropertyName("column")]
    public int Column { get; set; }
}

/// <summary>
/// Solving metadata
/// </summary>
public class SolvingMetadata
{
    /// <summary>
    /// Maximum recursion depth reached
    /// </summary>
    [JsonPropertyName("maxRecursionDepth")]
    public int MaxRecursionDepth { get; set; }

    /// <summary>
    /// Number of backtracking operations
    /// </summary>
    [JsonPropertyName("backtrackCount")]
    public int BacktrackCount { get; set; }

    /// <summary>
    /// Memory usage in bytes
    /// </summary>
    [JsonPropertyName("memoryUsageBytes")]
    public long MemoryUsageBytes { get; set; }
}

/// <summary>
/// Generation metadata
/// </summary>
public class GenerationMetadata
{
    /// <summary>
    /// Number of attempts to generate a valid puzzle
    /// </summary>
    [JsonPropertyName("attempts")]
    public int Attempts { get; set; }

    /// <summary>
    /// Whether the puzzle is unique
    /// </summary>
    [JsonPropertyName("isUnique")]
    public bool IsUnique { get; set; }

    /// <summary>
    /// Symmetry type used in generation
    /// </summary>
    [JsonPropertyName("symmetry")]
    public string Symmetry { get; set; } = "None";
} 