using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Api.Models;

/// <summary>
/// Request model for solving a Sudoku puzzle
/// </summary>
public class SolveSudokuRequest
{
    /// <summary>
    /// The Sudoku grid to solve
    /// </summary>
    [Required]
    [JsonPropertyName("grid")]
    public Grid Grid { get; set; } = new(new List<Cell>());

    /// <summary>
    /// Maximum time to spend solving in milliseconds (default: 30000)
    /// </summary>
    [Range(1000, 300000)]
    [JsonPropertyName("timeoutMs")]
    public int TimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Whether to include detailed solving steps
    /// </summary>
    [JsonPropertyName("includeSteps")]
    public bool IncludeSteps { get; set; } = false;
}

/// <summary>
/// Request model for validating a Sudoku grid
/// </summary>
public class ValidateSudokuRequest
{
    /// <summary>
    /// The Sudoku grid to validate
    /// </summary>
    [Required]
    [JsonPropertyName("grid")]
    public Grid Grid { get; set; } = new(new List<Cell>());

    /// <summary>
    /// Whether to perform complete validation (including solvability check)
    /// </summary>
    [JsonPropertyName("completeValidation")]
    public bool CompleteValidation { get; set; } = false;
}

/// <summary>
/// Request model for generating a new Sudoku puzzle
/// </summary>
public class GenerateSudokuRequest
{
    /// <summary>
    /// The desired difficulty level
    /// </summary>
    [Required]
    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = "Medium";

    /// <summary>
    /// The size of the Sudoku grid (default: 9)
    /// </summary>
    [Range(4, 16)]
    [JsonPropertyName("size")]
    public int Size { get; set; } = 9;

    /// <summary>
    /// Whether to include the solution in the response
    /// </summary>
    [JsonPropertyName("includeSolution")]
    public bool IncludeSolution { get; set; } = true;

    /// <summary>
    /// Maximum time to spend generating in milliseconds (default: 10000)
    /// </summary>
    [Range(1000, 60000)]
    [JsonPropertyName("timeoutMs")]
    public int TimeoutMs { get; set; } = 10000;
}

/// <summary>
/// Request model for getting solving hints
/// </summary>
public class GetHintRequest
{
    /// <summary>
    /// The current Sudoku grid state
    /// </summary>
    [Required]
    [JsonPropertyName("grid")]
    public Grid Grid { get; set; } = new(new List<Cell>());

    /// <summary>
    /// The row of the cell to get a hint for
    /// </summary>
    [Range(0, 8)]
    [JsonPropertyName("row")]
    public int Row { get; set; }

    /// <summary>
    /// The column of the cell to get a hint for
    /// </summary>
    [Range(0, 8)]
    [JsonPropertyName("column")]
    public int Column { get; set; }

    /// <summary>
    /// The type of hint to provide
    /// </summary>
    [JsonPropertyName("hintType")]
    public string HintType { get; set; } = "NextMove";
} 