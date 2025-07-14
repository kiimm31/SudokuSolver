using SudokuSolver.Core.Helpers;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Constrains;

public class BoxConstrain : Constrain
{
    public override string Name => nameof(BoxConstrain);
    public override Grid TrySolve(Grid grid, int referenceRow, int referenceColumn)
    {
        var referenceCell = grid.GetCell(referenceRow, referenceColumn);

        var box = grid.GetBox(referenceRow, referenceColumn);

        var rowLeftOverValue = PossibleValues.Clone();
        
        foreach (var cell in box.Where(cell => cell.IsSolved && !cell.Equals(referenceCell)))
        {
            rowLeftOverValue.Remove(cell.Value);
            referenceCell.EliminatePossibleValue(cell.Value);
        }

        return grid;
    }
}