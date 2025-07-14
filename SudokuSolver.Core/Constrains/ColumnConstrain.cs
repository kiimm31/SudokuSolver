using SudokuSolver.Core.Helpers;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constrains;

public class ColumnConstrain : Constrain
{
    public override string Name => nameof(ColumnConstrain);

    public override Grid TrySolve(Grid grid, int referenceRow, int referenceColumn)
    {
        var referenceCell = grid.GetCell(referenceRow, referenceColumn);
        
        var column = grid.GetColumn(referenceCell.Column);

        var rowLeftOverValue = PossibleValues.Clone();
        
        foreach (var cell in column.Where(cell => cell.IsSolved && !cell.Equals(referenceCell)))
        {
            rowLeftOverValue.Remove(cell.Value);
            referenceCell.EliminatePossibleValue(cell.Value);
        }

        return grid;
    }
}