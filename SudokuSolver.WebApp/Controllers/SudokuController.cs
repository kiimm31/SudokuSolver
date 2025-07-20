using Microsoft.AspNetCore.Mvc;
using SudokuSolver.Core.Factories;
using SudokuSolver.Domain.Models;
using System.Data;
using SudokuSolver.Core.Services;

namespace SudokuSolver.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SudokuController : ControllerBase
{
    private readonly ILogger<SudokuController> _logger;

    public SudokuController(ILogger<SudokuController> logger)
    {
        _logger = logger;
    }

    [HttpPost("solve")]
    public async Task<IActionResult> Solve([FromBody] SolveRequest request)
    {
        try
        {
            if (request.Grid == null || request.Grid.Length != 81)
            {
                return BadRequest(new { error = "Invalid grid format. Expected 81 cells (9x9 grid)." });
            }

            // Check if grid has enough values to be solvable (at least 17 clues are needed for a unique solution)
            int filledCells = request.Grid.Count(x => x > 0);
            if (filledCells < 17)
            {
                return BadRequest(new { error = "Puzzle needs at least 17 filled cells to have a unique solution. Current: " + filledCells });
            }

            // Convert the flat array to a grid with cells
            var cells = new List<Cell>();
            for (int row = 1; row <= 9; row++)
            {
                for (int col = 1; col <= 9; col++)
                {
                    int value = request.Grid[(row - 1) * 9 + (col - 1)];
                    var cell = new Cell { Row = row, Column = col, Value = value };
                    cells.Add(cell);
                }
            }
            var grid = new Grid(cells);

            // Validate the initial grid
            try
            {
                grid.ObeysAllConstraint();
            }
            catch (InvalidConstraintException)
            {
                return BadRequest(new { error = "Invalid Sudoku puzzle. The puzzle has no solution." });
            }

            // Solve the puzzle
            var solver = SudokuSolverFactory.CreateClassicSolver(grid);
            var solvedGrid = await Task.Run(() => solver.Solve());

            if (solvedGrid == null)
            {
                return BadRequest(new { error = "Unable to solve the puzzle. It may be too difficult or invalid." });
            }

            // Check if the solution is complete
            var solvedCells = solvedGrid.GetAllCells();
            var completedCells = solvedCells.Count(c => c.Value > 0);
            
            if (completedCells < 81)
            {
                return BadRequest(new { error = "Puzzle could not be completely solved. Only " + completedCells + " cells were filled." });
            }

            // Convert back to flat array
            var result = new int[81];
            for (int row = 1; row <= 9; row++)
            {
                for (int col = 1; col <= 9; col++)
                {
                    var cell = solvedGrid.GetCell(row, col);
                    result[(row - 1) * 9 + (col - 1)] = cell?.Value ?? 0;
                }
            }

            return Ok(new SolveResponse
            {
                Grid = result,
                IsSolved = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error solving Sudoku puzzle");
            return StatusCode(500, new { error = "An error occurred while solving the puzzle." });
        }
    }

    [HttpPost("validate")]
    public IActionResult Validate([FromBody] ValidateRequest request)
    {
        try
        {
            if (request.Grid == null || request.Grid.Length != 81)
            {
                return BadRequest(new { error = "Invalid grid format. Expected 81 cells (9x9 grid)." });
            }

            // Convert the flat array to a grid with cells
            var cells = new List<Cell>();
            for (int row = 1; row <= 9; row++)
            {
                for (int col = 1; col <= 9; col++)
                {
                    int value = request.Grid[(row - 1) * 9 + (col - 1)];
                    var cell = new Cell { Row = row, Column = col, Value = value };
                    cells.Add(cell);
                }
            }
            var grid = new Grid(cells);

            bool isValid;
            try
            {
                grid.ObeysAllConstraint();
                isValid = true;
            }
            catch (InvalidConstraintException)
            {
                isValid = false;
            }

            return Ok(new ValidateResponse
            {
                IsValid = isValid
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Sudoku puzzle");
            return StatusCode(500, new { error = "An error occurred while validating the puzzle." });
        }
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateRequest request)
    {
        try
        {
            // Validate difficulty level
            var validDifficulties = new[] { "easy", "medium", "hard", "expert" };
            if (!validDifficulties.Contains(request.Difficulty?.ToLower()))
            {
                return BadRequest(new { error = "Invalid difficulty. Must be: easy, medium, hard, or expert." });
            }

            // Generate puzzle based on difficulty
            var puzzle = await Task.Run(() => GeneratePuzzle(request.Difficulty));

            return Ok(new GenerateResponse
            {
                Grid = puzzle,
                Difficulty = request.Difficulty,
                Message = $"Generated {request.Difficulty} puzzle"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating Sudoku puzzle");
            return StatusCode(500, new { error = "An error occurred while generating the puzzle." });
        }
    }

    private int[] GeneratePuzzle(string difficulty)
    {
        // This is a placeholder implementation
        // You'll need to implement actual puzzle generation logic
        var random = new Random();
        var grid = new int[81];
        
        // Generate a solved grid first (you can use your solver with an empty grid)
        // Then remove numbers based on difficulty
        int cellsToRemove = difficulty.ToLower() switch
        {
            "easy" => 30,
            "medium" => 40,
            "hard" => 50,
            "expert" => 60,
            _ => 40
        };

        // For now, return a simple example puzzle
        return grid;
    }
}

public class SolveRequest
{
    public int[] Grid { get; set; } = Array.Empty<int>();
}

public class SolveResponse
{
    public int[] Grid { get; set; } = Array.Empty<int>();
    public bool IsSolved { get; set; }
}

public class ValidateRequest
{
    public int[] Grid { get; set; } = Array.Empty<int>();
}

public class ValidateResponse
{
    public bool IsValid { get; set; }
} 

public class GenerateRequest
{
    public string Difficulty { get; set; } = "medium";
}

public class GenerateResponse
{
    public int[] Grid { get; set; } = Array.Empty<int>();
    public string Difficulty { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
} 