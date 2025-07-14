using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constrains;

public class RowConstrain : Constrain
{
    public override string Name => nameof(RowConstrain);

    protected override List<Cell> GetInterestedCells(Grid grid, int referenceRow, int referenceColumn)
    {
        return grid.GetRow(referenceRow);
    }

    
}