using SudokuSolver.Core.Interfaces;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Base;

/// <summary>
/// Base class for solving strategy implementations providing common functionality
/// </summary>
public abstract class BaseSolvingStrategy : ISolvingStrategy
{
    protected static readonly List<int> PossibleValues = [1, 2, 3, 4, 5, 6, 7, 8, 9];
    
    public abstract string Name { get; }
    public abstract int Priority { get; }
    
    public virtual Grid Apply(Grid grid)
    {
        var unsolvedCells = grid.GetAllUnsolvedCells().ToList();
        
        // Apply strategy to rows
        foreach (var rowGroup in unsolvedCells.GroupBy(c => c.Row))
        {
            ApplyToGroup(rowGroup.ToList());
        }
        
        // Apply strategy to columns
        foreach (var colGroup in unsolvedCells.GroupBy(c => c.Column))
        {
            ApplyToGroup(colGroup.ToList());
        }
        
        // Apply strategy to boxes
        foreach (var boxGroup in unsolvedCells.GroupBy(c => c.GetBoxIndex()))
        {
            ApplyToGroup(boxGroup.ToList());
        }
        
        return grid;
    }
    
    public virtual bool CanApply(Grid grid)
    {
        return grid.GetAllUnsolvedCells().Any();
    }
    
    /// <summary>
    /// Applies the strategy to a specific group of cells (row, column, or box)
    /// </summary>
    /// <param name="cells">The cells in the group</param>
    protected abstract void ApplyToGroup(List<Cell> cells);
} 