using SudokuSolver.Core.Constraints;
using SudokuSolver.Core.Interfaces;
using SudokuSolver.Core.Services;
using SudokuSolver.Core.Strategies;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Factories;

/// <summary>
/// Factory for creating configured Sudoku solver instances
/// </summary>
public static class SudokuSolverFactory
{
    /// <summary>
    /// Creates a classic Sudoku solver with default constraints and strategies
    /// </summary>
    /// <param name="grid">The grid to solve</param>
    /// <returns>A configured ClassicSudokuSolver instance</returns>
    public static ISudokuSolver CreateClassicSolver(Grid grid)
    {
        var constraints = new List<IConstraint>
        {
            new RowConstraint(),
            new ColumnConstraint(),
            new BoxConstraint()
        };
        
        var strategies = new List<ISolvingStrategy>
        {
            new HiddenSinglesStrategy(),
            new NakedSubsetsStrategy(),
            new PointingStrategy()
        };
        
        return new ClassicSudokuSolver(grid, constraints, strategies);
    }
    
    /// <summary>
    /// Creates a classic Sudoku solver with custom constraints and strategies
    /// </summary>
    /// <param name="grid">The grid to solve</param>
    /// <param name="constraints">Custom constraints to use</param>
    /// <param name="strategies">Custom strategies to use</param>
    /// <returns>A configured ClassicSudokuSolver instance</returns>
    public static ISudokuSolver CreateClassicSolver(Grid grid, IEnumerable<IConstraint> constraints, IEnumerable<ISolvingStrategy> strategies)
    {
        return new ClassicSudokuSolver(grid, constraints, strategies);
    }
    
    /// <summary>
    /// Creates a basic Sudoku solver with only constraints (no advanced strategies)
    /// </summary>
    /// <param name="grid">The grid to solve</param>
    /// <returns>A configured ClassicSudokuSolver instance with only constraints</returns>
    public static ISudokuSolver CreateBasicSolver(Grid grid)
    {
        var constraints = new List<IConstraint>
        {
            new RowConstraint(),
            new ColumnConstraint(),
            new BoxConstraint()
        };
        
        return new ClassicSudokuSolver(grid, constraints, new List<ISolvingStrategy>());
    }
} 