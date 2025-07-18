using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Interfaces;

/// <summary>
/// Defines the contract for a Sudoku constraint
/// </summary>
public interface IConstraint
{
    /// <summary>
    /// Gets the name of the constraint
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Applies the constraint to the grid at the specified position
    /// </summary>
    /// <param name="grid">The grid to apply the constraint to</param>
    /// <param name="row">The row index (1-based)</param>
    /// <param name="column">The column index (1-based)</param>
    /// <returns>The modified grid</returns>
    Grid Apply(Grid grid, int row, int column);
    
    /// <summary>
    /// Validates that the constraint is satisfied at the specified position
    /// </summary>
    /// <param name="grid">The grid to validate</param>
    /// <param name="row">The row index (1-based)</param>
    /// <param name="column">The column index (1-based)</param>
    /// <returns>True if the constraint is satisfied, false otherwise</returns>
    bool IsSatisfied(Grid grid, int row, int column);
    
    /// <summary>
    /// Gets the cells that are affected by this constraint at the specified position
    /// </summary>
    /// <param name="grid">The grid</param>
    /// <param name="row">The row index (1-based)</param>
    /// <param name="column">The column index (1-based)</param>
    /// <returns>The list of cells affected by this constraint</returns>
    IReadOnlyList<Cell> GetAffectedCells(Grid grid, int row, int column);
} 