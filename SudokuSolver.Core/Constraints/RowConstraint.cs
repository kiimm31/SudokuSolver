using SudokuSolver.Core.Base;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constraints;

/// <summary>
/// Constraint that ensures each row contains unique values from 1-9
/// </summary>
public class RowConstraint : BaseConstraint
{
    public override string Name => "Row Constraint";
    
    public override IReadOnlyList<Cell> GetAffectedCells(Grid grid, int row, int column)
    {
        return grid.GetRow(row);
    }
} 