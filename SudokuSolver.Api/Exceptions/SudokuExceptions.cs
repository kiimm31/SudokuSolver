namespace SudokuSolver.Api.Exceptions;

/// <summary>
/// Base exception for Sudoku-related errors
/// </summary>
public abstract class SudokuException : Exception
{
    public string ErrorCode { get; }

    protected SudokuException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    protected SudokuException(string message, string errorCode, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Exception thrown when a Sudoku puzzle is unsolvable
/// </summary>
public class UnsolvablePuzzleException : SudokuException
{
    public UnsolvablePuzzleException(string message = "The Sudoku puzzle is unsolvable") 
        : base(message, "UNSOLVABLE_PUZZLE")
    {
    }
}

/// <summary>
/// Exception thrown when a Sudoku grid is invalid
/// </summary>
public class InvalidGridException : SudokuException
{
    public List<string> ValidationErrors { get; }

    public InvalidGridException(string message, List<string> validationErrors) 
        : base(message, "INVALID_GRID")
    {
        ValidationErrors = validationErrors;
    }
}

/// <summary>
/// Exception thrown when solving times out
/// </summary>
public class SolvingTimeoutException : SudokuException
{
    public int TimeoutMs { get; }

    public SolvingTimeoutException(int timeoutMs, string message = "Solving operation timed out") 
        : base(message, "SOLVING_TIMEOUT")
    {
        TimeoutMs = timeoutMs;
    }
}

/// <summary>
/// Exception thrown when puzzle generation fails
/// </summary>
public class PuzzleGenerationException : SudokuException
{
    public string Difficulty { get; }
    public int Attempts { get; }

    public PuzzleGenerationException(string difficulty, int attempts, string message = "Failed to generate puzzle") 
        : base(message, "PUZZLE_GENERATION_FAILED")
    {
        Difficulty = difficulty;
        Attempts = attempts;
    }
}

/// <summary>
/// Exception thrown when an invalid difficulty level is requested
/// </summary>
public class InvalidDifficultyException : SudokuException
{
    public string RequestedDifficulty { get; }
    public List<string> ValidDifficulties { get; }

    public InvalidDifficultyException(string requestedDifficulty, List<string> validDifficulties) 
        : base($"Invalid difficulty level: {requestedDifficulty}", "INVALID_DIFFICULTY")
    {
        RequestedDifficulty = requestedDifficulty;
        ValidDifficulties = validDifficulties;
    }
}

/// <summary>
/// Exception thrown when a hint cannot be provided
/// </summary>
public class HintUnavailableException : SudokuException
{
    public int Row { get; }
    public int Column { get; }

    public HintUnavailableException(int row, int column, string message = "Hint unavailable for the specified cell") 
        : base(message, "HINT_UNAVAILABLE")
    {
        Row = row;
        Column = column;
    }
} 