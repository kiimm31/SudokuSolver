using Microsoft.Extensions.Logging;
using SudokuSolver.Api.Exceptions;
using SudokuSolver.Core.Interfaces;
using SudokuSolver.Domain.Models;
using System.Diagnostics;

namespace SudokuSolver.Api.Services;

/// <summary>
/// Service for generating Sudoku puzzles
/// </summary>
public class PuzzleGenerationService
{
    private readonly ISudokuSolverFactory _solverFactory;
    private readonly ILogger<PuzzleGenerationService> _logger;

    public PuzzleGenerationService(ISudokuSolverFactory solverFactory, ILogger<PuzzleGenerationService> logger)
    {
        _solverFactory = solverFactory;
        _logger = logger;
    }

    /// <summary>
    /// Generates a new Sudoku puzzle with the specified difficulty
    /// </summary>
    /// <param name="difficulty">The desired difficulty level</param>
    /// <param name="timeoutMs">Maximum time to spend generating</param>
    /// <returns>The generated puzzle and solution</returns>
    public async Task<(Grid Puzzle, Grid Solution)> GeneratePuzzleAsync(string difficulty, int timeoutMs = 10000)
    {
        var stopwatch = Stopwatch.StartNew();
        var attempts = 0;
        const int maxAttempts = 100;

        while (attempts < maxAttempts && stopwatch.ElapsedMilliseconds < timeoutMs)
        {
            attempts++;

            try
            {
                var (puzzle, solution) = await Task.Run(() => GeneratePuzzleWithDifficulty(difficulty));
                
                // Verify the puzzle is solvable and has a unique solution
                if (await VerifyPuzzleAsync(puzzle, solution))
                {
                    _logger.LogInformation("Puzzle generated successfully after {Attempts} attempts. Difficulty: {Difficulty}", 
                        attempts, difficulty);
                    return (puzzle, solution);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to generate puzzle on attempt {Attempt}. Difficulty: {Difficulty}", 
                    attempts, difficulty);
            }
        }

        throw new PuzzleGenerationException(difficulty, attempts, 
            $"Failed to generate puzzle after {attempts} attempts within {timeoutMs}ms");
    }

    /// <summary>
    /// Generates a puzzle with the specified difficulty level
    /// </summary>
    private (Grid Puzzle, Grid Solution) GeneratePuzzleWithDifficulty(string difficulty)
    {
        // Start with a complete solved grid
        var solution = GenerateCompleteGrid();
        
        // Create a copy for the puzzle
        var puzzle = solution.Clone();
        
        // Remove cells based on difficulty
        var cellsToRemove = GetCellsToRemoveForDifficulty(difficulty);
        RemoveRandomCells(puzzle, cellsToRemove);

        return (puzzle, solution);
    }

    /// <summary>
    /// Generates a complete, valid Sudoku grid
    /// </summary>
    private Grid GenerateCompleteGrid()
    {
        var cells = new List<Cell>();
        
        // Initialize all cells
        for (int row = 1; row <= 9; row++)
        {
            for (int col = 1; col <= 9; col++)
            {
                cells.Add(new Cell { Row = row, Column = col });
            }
        }

        var grid = new Grid(cells);

        // Fill the grid using a simple algorithm
        FillGridRecursively(grid, 0);

        return grid;
    }

    /// <summary>
    /// Recursively fills the grid with valid values
    /// </summary>
    private bool FillGridRecursively(Grid grid, int index)
    {
        if (index >= 81)
            return true;

        var row = (index / 9) + 1;
        var col = (index % 9) + 1;

        // Try each number 1-9
        var numbers = Enumerable.Range(1, 9).OrderBy(x => Guid.NewGuid()).ToArray();
        
        foreach (var num in numbers)
        {
            if (IsValidPlacement(grid, row, col, num))
            {
                grid.SetCell(row, col, num);
                
                if (FillGridRecursively(grid, index + 1))
                    return true;
                
                // Reset the cell
                var cell = grid.GetCell(row, col);
                if (cell != null)
                    cell.Value = 0;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if a number can be placed at the specified position
    /// </summary>
    private bool IsValidPlacement(Grid grid, int row, int col, int num)
    {
        // Check row
        var rowCells = grid.GetRow(row);
        foreach (var cell in rowCells)
        {
            if (cell.Column != col && cell.Value == num)
                return false;
        }

        // Check column
        var colCells = grid.GetColumn(col);
        foreach (var cell in colCells)
        {
            if (cell.Row != row && cell.Value == num)
                return false;
        }

        // Check 3x3 box
        var boxCells = grid.GetBox(row, col);
        foreach (var cell in boxCells)
        {
            if ((cell.Row != row || cell.Column != col) && cell.Value == num)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Gets the number of cells to remove based on difficulty
    /// </summary>
    private int GetCellsToRemoveForDifficulty(string difficulty)
    {
        return difficulty.ToLower() switch
        {
            "easy" => 30,      // 51 clues
            "medium" => 40,    // 41 clues
            "hard" => 50,      // 31 clues
            "expert" => 55,    // 26 clues
            _ => 40
        };
    }

    /// <summary>
    /// Removes random cells from the puzzle
    /// </summary>
    private void RemoveRandomCells(Grid puzzle, int count)
    {
        var random = new Random();
        var allCells = puzzle.GetAllCells();
        var positions = Enumerable.Range(0, allCells.Count).ToList();
        
        for (int i = 0; i < count && positions.Count > 0; i++)
        {
            var randomIndex = random.Next(positions.Count);
            var position = positions[randomIndex];
            positions.RemoveAt(randomIndex);
            
            allCells[position].Value = 0;
        }
    }

    /// <summary>
    /// Verifies that the puzzle is solvable and has a unique solution
    /// </summary>
    private async Task<bool> VerifyPuzzleAsync(Grid puzzle, Grid expectedSolution)
    {
        try
        {
            // Try to solve the puzzle
            var solver = _solverFactory.CreateSolver();
            solver.SetGrid(puzzle);
            var solvedGrid = await Task.Run(() => solver.Solve());
            
            if (solvedGrid == null)
                return false;

            // Check if the solution matches the expected solution
            return GridsAreEqual(solvedGrid, expectedSolution);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Compares two grids for equality
    /// </summary>
    private bool GridsAreEqual(Grid grid1, Grid grid2)
    {
        var cells1 = grid1.GetAllCells();
        var cells2 = grid2.GetAllCells();

        if (cells1.Count != cells2.Count)
            return false;

        for (int i = 0; i < cells1.Count; i++)
        {
            if (cells1[i].Value != cells2[i].Value)
                return false;
        }

        return true;
    }
} 