using SudokuSolver.Core.Base;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constraints;

/// <summary>
/// Constraint that ensures each 3x3 box contains unique values from 1-9
/// </summary>
public class BoxConstraint : BaseConstraint
{
    public override string Name => "Box Constraint";
    
    public override IReadOnlyList<Cell> GetAffectedCells(Grid grid, int row, int column)
    {
        return grid.GetBox(row, column);
    }
} 