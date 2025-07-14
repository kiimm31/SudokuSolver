using SudokuSolver.Core.Helpers;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constrains;

public class BoxConstrain : Constrain
{
    public override string Name => nameof(BoxConstrain);

    protected override List<Cell> GetInterestedCells(Grid grid, int referenceRow, int referenceColumn)
    {
        return grid.GetBox(referenceRow, referenceColumn);
    }
}