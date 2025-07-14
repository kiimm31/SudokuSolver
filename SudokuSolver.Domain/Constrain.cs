using SudokuSolver.Domain.Models;

namespace SudokuSolver.Domain;

public abstract class Constrain
{
    public abstract string Name { get; }

    private static readonly List<int> PossibleValues = [1, 2, 3, 4, 5, 6, 7, 8, 9];

    public virtual Grid TrySolve(Grid grid, int referenceRow, int referenceColumn)
    {
        // get reference cell
        var referenceCell = GetReferenceCell(grid, referenceRow, referenceColumn);
        
        // get interested cells based on the type of constraint
        var interestedCells = GetInterestedCells(grid, referenceRow, referenceColumn);
        
        //clone possible values
        var possibleValues = new List<int>();
        possibleValues.AddRange(PossibleValues);
        
        // eliminate possible values based on the reference cell
        EliminateReferenceCellsPossibleValues(interestedCells, referenceCell, possibleValues);
        
        // return the modified grid
        return grid;
    }

    private Cell GetReferenceCell(Grid grid, int referenceRow, int referenceColumn)
    {
        return grid.GetCell(referenceRow, referenceColumn);
    }

    protected abstract List<Cell> GetInterestedCells(Grid grid, int referenceRow, int referenceColumn);
    
    protected virtual void EliminateReferenceCellsPossibleValues(List<Cell> interestedCells, Cell referenceCell, List<int> possibleValues)
    {
        foreach (var cell in interestedCells.Where(cell => cell.IsSolved && !cell.Equals(referenceCell)))
        {
            possibleValues.Remove(cell.Value);
            referenceCell.EliminatePossibleValue(cell.Value);
        }
    }
    
    
    
}