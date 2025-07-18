using SudokuSolver.Core.Interfaces;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Base;

/// <summary>
/// Base class for constraint implementations providing common functionality
/// </summary>
public abstract class BaseConstraint : IConstraint
{
    protected static readonly List<int> PossibleValues = [1, 2, 3, 4, 5, 6, 7, 8, 9];
    
    public abstract string Name { get; }
    
    public virtual Grid Apply(Grid grid, int row, int column)
    {
        var referenceCell = grid.GetCell(row, column);
        if (referenceCell == null)
            return grid;
            
        var affectedCells = GetAffectedCells(grid, row, column);
        EliminatePossibleValues(affectedCells, referenceCell);
        
        return grid;
    }
    
    public virtual bool IsSatisfied(Grid grid, int row, int column)
    {
        var cells = GetAffectedCells(grid, row, column);
        var solvedValues = cells.Where(c => c.IsSolved).Select(c => c.Value).ToList();
        
        // Check if there are any duplicate values in the solved cells
        return solvedValues.Distinct().Count() == solvedValues.Count;
    }
    
    public abstract IReadOnlyList<Cell> GetAffectedCells(Grid grid, int row, int column);
    
    /// <summary>
    /// Eliminates possible values from the reference cell based on solved cells in the affected group
    /// </summary>
    /// <param name="affectedCells">Cells affected by this constraint</param>
    /// <param name="referenceCell">The cell to eliminate values from</param>
    protected virtual void EliminatePossibleValues(IReadOnlyList<Cell> affectedCells, Cell referenceCell)
    {
        if (referenceCell.IsSolved)
            return;
            
        foreach (var cell in affectedCells.Where(c => c.IsSolved && !c.Equals(referenceCell)))
        {
            referenceCell.EliminatePossibleValue(cell.Value);
        }
    }
} 