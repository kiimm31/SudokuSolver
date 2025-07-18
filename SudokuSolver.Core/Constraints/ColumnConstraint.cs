using SudokuSolver.Core.Base;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constraints;

/// <summary>
/// Constraint that ensures each column contains unique values from 1-9
/// </summary>
public class ColumnConstraint : BaseConstraint
{
    public override string Name => "Column Constraint";
    
    public override IReadOnlyList<Cell> GetAffectedCells(Grid grid, int row, int column)
    {
        return grid.GetColumn(column);
    }
} 