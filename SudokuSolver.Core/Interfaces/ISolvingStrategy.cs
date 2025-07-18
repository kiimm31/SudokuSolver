using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Interfaces;

/// <summary>
/// Defines the contract for a Sudoku solving strategy
/// </summary>
public interface ISolvingStrategy
{
    /// <summary>
    /// Gets the name of the strategy
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the priority of the strategy (lower numbers = higher priority)
    /// </summary>
    int Priority { get; }
    
    /// <summary>
    /// Applies the strategy to solve the grid
    /// </summary>
    /// <param name="grid">The grid to apply the strategy to</param>
    /// <returns>The modified grid</returns>
    Grid Apply(Grid grid);
    
    /// <summary>
    /// Checks if the strategy can be applied to the current grid state
    /// </summary>
    /// <param name="grid">The grid to check</param>
    /// <returns>True if the strategy can be applied, false otherwise</returns>
    bool CanApply(Grid grid);
} 