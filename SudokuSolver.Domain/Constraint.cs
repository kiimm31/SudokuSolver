using System.Data;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Domain;

public abstract class Constraint
{
    public abstract string Name { get; }

    private static readonly List<int> PossibleValues = [1, 2, 3, 4, 5, 6, 7, 8, 9];

    private Grid referenceGrid;

    public virtual Grid TrySolve(Grid grid, int referenceRow, int referenceColumn)
    {
        // get reference cell
        var referenceCell = GetReferenceCell(grid, referenceRow, referenceColumn);
        
        // get interested cells based on the type of constraint
        var interestedCells = GetInterestedCells(grid, referenceRow, referenceColumn);

        // eliminate possible values based on the reference cell
        referenceGrid = grid;
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
        foreach (var cell in interestedCells.Where(cell => cell.IsSolved && !cell.Equals(referenceCell)))
        {
            referenceCell.EliminatePossibleValue(cell.Value);
            
            // return the modified grid
            if (!ObeysConstraint(referenceGrid, referenceCell.Row, referenceCell.Column))
            {
                Console.WriteLine(ToResult(referenceGrid, referenceCell.Row, referenceCell.Column));
                throw new InvalidConstraintException(
                    $"Grid is invalid due to {Name} violation at cell ({referenceCell.Row}, {referenceCell.Column}).\n{referenceGrid}");
            }
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