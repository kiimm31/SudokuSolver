using SudokuSolver.Core.Helpers;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constrains;

public class RowConstraint : Constraint
{
    public override string Name => nameof(RowConstraint);

    protected override List<Cell> GetInterestedCells(Grid grid, int referenceRow, int referenceColumn)
    {
        return grid.GetRow(referenceRow);
    }

    public override string ToResult(Grid grid, int referenceRow, int referenceColumn)
    {
        return grid.GetRow(referenceRow).ToRow();
    }
}