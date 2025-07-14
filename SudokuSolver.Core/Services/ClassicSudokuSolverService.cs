using SudokuSolver.Core.Constrains;
using SudokuSolver.Core.Helpers;
using SudokuSolver.Core.Interface;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Services;

public class ClassicSudokuSolverService(Grid grid) : AbstractSudokuSolverService
{
    public override Grid MyGrid { get; set; } = grid;
    private const int MaxUnchangedIterations = 3;

    public override List<Constrain> Constrains { get; init; } =
    [
        new RowConstrain(),
        new ColumnConstrain(),
        new BoxConstrain()
    ];

    public override Grid Solve()
    {
        if (MyGrid is null)
        {
            throw new ArgumentNullException(nameof(grid));
        }

        var unchangedIterations = 0;

        while (!MyGrid.IsSolved())
        {
            var previousGrid = MyGrid.Clone(); // Assuming Grid has a Clone method

            foreach (var constrain in Constrains)
            {
                foreach (var referenceCell in MyGrid.GetAllUnsolvedCells())
                {
                    MyGrid = constrain.TrySolve(MyGrid, referenceCell.Row, referenceCell.Column);
                }
            }

            // Check if the grid has changed
            if (MyGrid.Equals(previousGrid))
            {
                unchangedIterations++;
                if (unchangedIterations >= MaxUnchangedIterations)
                {
                    break; // Exit the loop if no changes for 3 iterations
                }
            }
            else
            {
                unchangedIterations = 0; // Reset counter if grid changed
            }
        }

        return MyGrid;
    }
}