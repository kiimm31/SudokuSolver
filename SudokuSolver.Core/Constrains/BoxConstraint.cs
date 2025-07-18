using SudokuSolver.Core.Helpers;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constrains;

public class BoxConstraint : Constraint
{
    public override string Name => nameof(BoxConstraint);

    protected override List<Cell> GetInterestedCells(Grid grid, int referenceRow, int referenceColumn)
    {
        return grid.GetBox(referenceRow, referenceColumn);
    }

    public override string ToResult(Grid grid, int referenceRow, int referenceColumn)
    {
        return grid.GetBox(referenceRow, referenceColumn).ToBox();
    }
}