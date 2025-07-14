using SudokuSolver.Core.Helpers;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constrains;

public class ColumnConstrain : Constrain
{
    public override string Name => nameof(ColumnConstrain);

    protected override List<Cell> GetInterestedCells(Grid grid, int referenceRow, int referenceColumn)
    {
        return grid.GetColumn(referenceColumn);
    }
}