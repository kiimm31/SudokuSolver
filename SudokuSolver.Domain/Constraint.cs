using System.Data;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Domain;

public abstract class Constraint
{
    public abstract string Name { get; }

    private static readonly List<int> PossibleValues = [1, 2, 3, 4, 5, 6, 7, 8, 9];

    public virtual Grid TrySolve(Grid grid, int referenceRow, int referenceColumn)
    {
        // get reference cell
        var referenceCell = GetReferenceCell(grid, referenceRow, referenceColumn);
        
        // get interested cells based on the type of constraint
        var interestedCells = GetInterestedCells(grid, referenceRow, referenceColumn);

        // eliminate possible values based on the reference cell
        EliminateReferenceCellsPossibleValues(interestedCells, referenceCell);

       return grid;
    }

    private Cell GetReferenceCell(Grid grid, int referenceRow, int referenceColumn)
    {
        return grid.GetCell(referenceRow, referenceColumn)!;
    }

    protected abstract List<Cell> GetInterestedCells(Grid grid, int referenceRow, int referenceColumn);

    protected virtual void EliminateReferenceCellsPossibleValues(List<Cell> interestedCells, Cell referenceCell)
    {
        // Only eliminate values if the reference cell is not solved
        if (referenceCell.IsSolved)
            return;
            
        foreach (var cell in interestedCells.Where(cell => cell.IsSolved && !cell.Equals(referenceCell)))
        {
            referenceCell.EliminatePossibleValue(cell.Value);
        }
    }

    public virtual bool ObeysConstraint(Grid grid, int referenceRow, int referenceColumn)
    {
        var cells = GetInterestedCells(grid, referenceRow, referenceColumn);

        return CheckValidity(cells);
    }

    private bool CheckValidity(List<Cell> cells)
    {
        var solvedCellsValues = cells.Where(x => x.IsSolved)
            .Select(x => x.Value)
            .ToList();
        return solvedCellsValues.Distinct().Count() == solvedCellsValues.Count;
    }

    public abstract string ToResult(Grid grid, int referenceRow, int referenceColumn);
}