using System.Data;
using SudokuSolver.Core.Constrains;
using SudokuSolver.Domain.Models;
using Constraint = SudokuSolver.Domain.Constraint;

namespace SudokuSolver.Core.Services;

public static class GridValidator
{
    private static readonly List<Constraint> Constrains =
    [
        new RowConstraint(),
        new ColumnConstraint(),
        new BoxConstraint()
    ];

    public static bool ObeysAllConstraint(this Grid grid)
    {
        foreach (var constraint in Constrains)
        {
            grid.Obeys(constraint);
        }

        return true;
    }

    public static void Obeys(this Grid grid, Constraint constraint)
    {
        foreach (var cell in grid.GetSolvedCells())
        {
            if (!constraint.ObeysConstraint(grid, cell.Row, cell.Column))
            {
                Console.WriteLine(constraint.ToResult(grid, cell.Row, cell.Column));
                throw new InvalidConstraintException(
                    $"Grid is invalid due to {constraint.Name} violation at cell ({cell.Row}, {cell.Column}).\n{grid}");
            }
        }
    }
}