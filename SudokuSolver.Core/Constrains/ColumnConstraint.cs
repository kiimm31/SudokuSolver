using SudokuSolver.Core.Helpers;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constrains;

public class ColumnConstraint : Constraint
{
    public override string Name => nameof(ColumnConstraint);

    protected override List<Cell> GetInterestedCells(Grid grid, int referenceRow, int referenceColumn)
    {
        return grid.GetColumn(referenceColumn);
    }

    public override string ToResult(Grid grid, int referenceRow, int referenceColumn)
    {
        return grid.GetColumn(referenceColumn).ToColumn();
    }
}