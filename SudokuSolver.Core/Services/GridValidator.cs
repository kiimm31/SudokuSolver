using System.Data;
using SudokuSolver.Core.Constraints;
using SudokuSolver.Core.Interfaces;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Services;

public static class GridValidator
{
    private static readonly List<IConstraint> Constraints =
    [
        new RowConstraint(),
        new ColumnConstraint(),
        new BoxConstraint()
    ];

    public static bool ObeysAllConstraint(this Grid grid)
    {
        foreach (var constraint in Constraints)
        {
            grid.Obeys(constraint);
        }

        return true;
    }

    public static void Obeys(this Grid grid, IConstraint constraint)
    {
        foreach (var cell in grid.GetSolvedCells())
        {
            if (!constraint.IsSatisfied(grid, cell.Row, cell.Column))
            {
                throw new InvalidConstraintException(
                    $"Grid is invalid due to {constraint.Name} violation at cell ({cell.Row}, {cell.Column}).\n{grid}");
            }
        }
    }
}