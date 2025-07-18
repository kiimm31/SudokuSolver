using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Interfaces;

/// <summary>
/// Defines the contract for a Sudoku solver implementation
/// </summary>
public interface ISudokuSolver
{
    /// <summary>
    /// Gets the current grid being solved
    /// </summary>
    Grid Grid { get; }
    
    /// <summary>
    /// Gets the list of constraints applied by this solver
    /// </summary>
    IReadOnlyList<IConstraint> Constraints { get; }
    
    /// <summary>
    /// Gets the list of solving strategies used by this solver
    /// </summary>
    IReadOnlyList<ISolvingStrategy> Strategies { get; }
    
    /// <summary>
    /// Solves the Sudoku puzzle and returns the solved grid
    /// </summary>
    /// <returns>The solved grid</returns>
    Grid Solve();
    
    /// <summary>
    /// Sets the grid to be solved
    /// </summary>
    /// <param name="grid">The grid to solve</param>
    void SetGrid(Grid grid);
} 