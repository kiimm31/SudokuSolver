using SudokuSolver.Core.Helpers;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constrains;

public class RowConstrain : Constrain
{
    public override string Name => nameof(RowConstrain);

    public override Grid TrySolve(Grid grid, int referenceRow, int referenceColumn)
    {
        var referenceCell = grid.GetCell(referenceRow, referenceColumn);
        
        var rows = grid.GetRow(referenceCell.Row);

        var rowLeftOverValue = PossibleValues.Clone();
        
        foreach (var cell in rows.Where(cell => cell.IsSolved && !cell.Equals(referenceCell)))
        {
            rowLeftOverValue.Remove(cell.Value);
            referenceCell.EliminatePossibleValue(cell.Value);
        }

        return grid;
    }
}