using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SudokuSolver.Api.Exceptions;
using SudokuSolver.Api.Models;
using SudokuSolver.Api.Services;
using SudokuSolver.Core.Interfaces;
using SudokuSolver.Core.Services;
using SudokuSolver.Domain.Models;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace SudokuSolver.Api.Controllers;

/// <summary>
/// Controller for Sudoku solving, validation, and generation operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SudokuController(
    ISudokuSolverFactory solverFactory,
    PuzzleGenerationService generationService,
    ILogger<SudokuController> logger)
    : ControllerBase
{
    /// <summary>
    /// Solves a Sudoku puzzle
    /// </summary>
    /// <param name="request">The solve request containing the grid and options</param>
    /// <returns>The solved grid or error if unsolvable</returns>
    [HttpPost("solve")]
    [ProducesResponseType(typeof(ApiResponse<SudokuSolveResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 422)]
    [ProducesResponseType(typeof(ApiResponse<object>), 408)]
    public async Task<ActionResult<ApiResponse<SudokuSolveResponse>>> Solve([FromBody] SolveSudokuRequest request)
    {
        var correlationId = HttpContext.TraceIdentifier;
        logger.LogInformation("Solve request received. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            // Validate input grid
            ValidateGrid(request.Grid);

            var stopwatch = Stopwatch.StartNew();
            var originalGrid = request.Grid.Clone();

            // Create a solver and attempt to solve the puzzle
            var solver = solverFactory.CreateSolver();
            solver.SetGrid(request.Grid);
            var solvedGrid = await Task.Run(() => solver.Solve());

            stopwatch.Stop();

            var response = new SudokuSolveResponse
            {
                OriginalGrid = originalGrid,
                SolvedGrid = solvedGrid,
                IsSolvable = solvedGrid.IsSolved(),
                SolveTimeMs = stopwatch.ElapsedMilliseconds,
                StepsTaken = 0, // TODO: Implement step tracking
                StrategiesUsed = new List<string>(), // TODO: Implement strategy tracking
                Difficulty = AssessDifficulty(originalGrid),
                Metadata = new SolvingMetadata
                {
                    MaxRecursionDepth = 0, // TODO: Implement tracking
                    BacktrackCount = 0, // TODO: Implement tracking
                    MemoryUsageBytes = GC.GetTotalMemory(false)
                }
            };

            if (solvedGrid == null)
            {
                throw new UnsolvablePuzzleException();
            }

            logger.LogInformation("Puzzle solved successfully. Time: {Time}ms. CorrelationId: {CorrelationId}", 
                stopwatch.ElapsedMilliseconds, correlationId);

            return Ok(ApiResponse<SudokuSolveResponse>.SuccessResponse(response, correlationId));
        }
        catch (SudokuException)
        {
            throw; // Let the global exception handler deal with it
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during solving. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }

    /// <summary>
    /// Validates a Sudoku grid
    /// </summary>
    /// <param name="request">The validation request</param>
    /// <returns>Validation results</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ApiResponse<SudokuValidationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public ActionResult<ApiResponse<SudokuValidationResponse>> Validate([FromBody] ValidateSudokuRequest request)
    {
        var correlationId = HttpContext.TraceIdentifier;
        logger.LogInformation("Validation request received. CorrelationId: {CorrelationId}", correlationId);

        try
        {
            var errors = new List<ValidationError>();
            var isValid = true;

            // Basic grid structure validation
            var allCells = request.Grid.GetAllCells();
            if (allCells == null || allCells.Count != 81)
            {
                errors.Add(new ValidationError
                {
                    Type = "GridStructure",
                    Message = "Grid must have exactly 81 cells (9x9)"
                });
                isValid = false;
            }

            if (isValid)
            {
                // Validate each cell
                foreach (var cell in allCells)
                {
                    if (cell.Value != 0 && (cell.Value < 1 || cell.Value > 9))
                    {
                        errors.Add(new ValidationError
                        {
                            Type = "InvalidValue",
                            Message = $"Cell value must be between 1 and 9, got {cell.Value}",
                            Position = new CellPosition { Row = cell.Row - 1, Column = cell.Column - 1 }
                        });
                        isValid = false;
                    }
                }

                // Check for duplicate values in rows, columns, and boxes
                if (isValid)
                {
                    var rowErrors = CheckRowDuplicates(request.Grid);
                    var colErrors = CheckColumnDuplicates(request.Grid);
                    var boxErrors = CheckBoxDuplicates(request.Grid);

                    errors.AddRange(rowErrors);
                    errors.AddRange(colErrors);
                    errors.AddRange(boxErrors);

                    isValid = errors.Count == 0;
                }
            }

            var filledCells = allCells.Count(c => c.Value != 0);
            var emptyCells = 81 - filledCells;

            var response = new SudokuValidationResponse
            {
                IsValid = isValid,
                Errors = errors,
                IsComplete = emptyCells == 0,
                FilledCells = filledCells,
                EmptyCells = emptyCells
            };

            return Ok(ApiResponse<SudokuValidationResponse>.SuccessResponse(response, correlationId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during validation. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }

    /// <summary>
    /// Generates a new Sudoku puzzle
    /// </summary>
    /// <param name="request">The generation request</param>
    /// <returns>The generated puzzle and solution</returns>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(ApiResponse<SudokuGenerationResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<ActionResult<ApiResponse<SudokuGenerationResponse>>> Generate([FromBody] GenerateSudokuRequest request)
    {
        var correlationId = HttpContext.TraceIdentifier;
        logger.LogInformation("Generation request received. Difficulty: {Difficulty}. CorrelationId: {CorrelationId}", 
            request.Difficulty, correlationId);

        try
        {
            // Validate difficulty level
            var validDifficulties = new List<string> { "Easy", "Medium", "Hard", "Expert" };
            if (!validDifficulties.Contains(request.Difficulty, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidDifficultyException(request.Difficulty, validDifficulties);
            }

            var stopwatch = Stopwatch.StartNew();

            // Generate puzzle using the service
            var (puzzle, solution) = await generationService.GeneratePuzzleAsync(request.Difficulty, request.TimeoutMs);

            stopwatch.Stop();

            var response = new SudokuGenerationResponse
            {
                Puzzle = puzzle,
                Solution = request.IncludeSolution ? solution : new Grid(new List<Cell>()),
                Difficulty = request.Difficulty,
                GenerationTimeMs = stopwatch.ElapsedMilliseconds,
                ClueCount = puzzle.GetAllCells()?.Count(c => c.Value != 0) ?? 0,
                Metadata = new GenerationMetadata
                {
                    Attempts = 1,
                    IsUnique = true,
                    Symmetry = "None"
                }
            };

            logger.LogInformation("Puzzle generated successfully. Time: {Time}ms. CorrelationId: {CorrelationId}", 
                stopwatch.ElapsedMilliseconds, correlationId);

            return Ok(ApiResponse<SudokuGenerationResponse>.SuccessResponse(response, correlationId));
        }
        catch (SudokuException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during generation. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }

    /// <summary>
    /// Gets a hint for a specific cell
    /// </summary>
    /// <param name="request">The hint request</param>
    /// <returns>The hint information</returns>
    [HttpPost("hint")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public ActionResult<ApiResponse<object>> GetHint([FromBody] GetHintRequest request)
    {
        var correlationId = HttpContext.TraceIdentifier;
        logger.LogInformation("Hint request received for cell ({Row}, {Column}). CorrelationId: {CorrelationId}", 
            request.Row, request.Column, correlationId);

        try
        {
            ValidateGrid(request.Grid);

            // Check if cell is empty
            var cell = request.Grid.GetCell(request.Row + 1, request.Column + 1);
            if (cell?.Value != 0)
            {
                throw new HintUnavailableException(request.Row, request.Column, "Cell is already filled");
            }

            // TODO: Implement actual hint logic
            var hint = new
            {
                Row = request.Row,
                Column = request.Column,
                SuggestedValue = 1, // Placeholder
                Reason = "This is a placeholder hint"
            };

            return Ok(ApiResponse<object>.SuccessResponse(hint, correlationId));
        }
        catch (SudokuException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during hint generation. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public ActionResult<ApiResponse<object>> Health()
    {
        var health = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        };

        return Ok(ApiResponse<object>.SuccessResponse(health, HttpContext.TraceIdentifier));
    }

    #region Private Methods

    private void ValidateGrid(Grid grid)
    {
        if (grid == null)
        {
            throw new InvalidGridException("Grid cannot be null", new List<string> { "Grid is null" });
        }

        var allCells = grid.GetAllCells();
        if (allCells == null || allCells.Count != 81)
        {
            throw new InvalidGridException("Invalid grid structure", new List<string> { "Grid must have exactly 81 cells" });
        }
    }

    private List<ValidationError> CheckRowDuplicates(Grid grid)
    {
        var errors = new List<ValidationError>();
        
        for (int row = 1; row <= 9; row++)
        {
            var rowCells = grid.GetRow(row);
            var values = new HashSet<int>();
            
            foreach (var cell in rowCells)
            {
                if (cell.Value != 0)
                {
                    if (!values.Add(cell.Value))
                    {
                        errors.Add(new ValidationError
                        {
                            Type = "RowDuplicate",
                            Message = $"Duplicate value {cell.Value} in row {row}",
                            Position = new CellPosition { Row = cell.Row - 1, Column = cell.Column - 1 }
                        });
                    }
                }
            }
        }

        return errors;
    }

    private List<ValidationError> CheckColumnDuplicates(Grid grid)
    {
        var errors = new List<ValidationError>();
        
        for (int col = 1; col <= 9; col++)
        {
            var colCells = grid.GetColumn(col);
            var values = new HashSet<int>();
            
            foreach (var cell in colCells)
            {
                if (cell.Value != 0)
                {
                    if (!values.Add(cell.Value))
                    {
                        errors.Add(new ValidationError
                        {
                            Type = "ColumnDuplicate",
                            Message = $"Duplicate value {cell.Value} in column {col}",
                            Position = new CellPosition { Row = cell.Row - 1, Column = cell.Column - 1 }
                        });
                    }
                }
            }
        }

        return errors;
    }

    private List<ValidationError> CheckBoxDuplicates(Grid grid)
    {
        var errors = new List<ValidationError>();
        
        for (int boxRow = 1; boxRow <= 9; boxRow += 3)
        {
            for (int boxCol = 1; boxCol <= 9; boxCol += 3)
            {
                var boxCells = grid.GetBox(boxRow, boxCol);
                var values = new HashSet<int>();
                
                foreach (var cell in boxCells)
                {
                    if (cell.Value != 0)
                    {
                        if (!values.Add(cell.Value))
                        {
                            errors.Add(new ValidationError
                            {
                                Type = "BoxDuplicate",
                                Message = $"Duplicate value {cell.Value} in box containing cell ({boxRow}, {boxCol})",
                                Position = new CellPosition { Row = cell.Row - 1, Column = cell.Column - 1 }
                            });
                        }
                    }
                }
            }
        }

        return errors;
    }

    private string AssessDifficulty(Grid grid)
    {
        var allCells = grid.GetAllCells();
        var filledCells = allCells.Count(c => c.Value != 0);
        var emptyCells = 81 - filledCells;

        return emptyCells switch
        {
            <= 20 => "Easy",
            <= 35 => "Medium",
            <= 50 => "Hard",
            _ => "Expert"
        };
    }



    #endregion
} 