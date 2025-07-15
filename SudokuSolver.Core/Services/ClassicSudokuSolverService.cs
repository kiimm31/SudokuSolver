using SudokuSolver.Core.Constrains;
using SudokuSolver.Core.Helpers;
using SudokuSolver.Core.Interface;
using SudokuSolver.Core.Strategies;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Services;

public class ClassicSudokuSolverService(Grid grid) : AbstractSudokuSolverService
{
    public override Grid MyGrid { get; set; } = grid;
    public override List<Constrain> Constrains { get; init; } =
    [
        new RowConstrain(),
        new ColumnConstrain(),
        new BoxConstrain()
    ];
    
    private int MaxUnchangedIterations { get; init; } = 5;
    
    public override List<Strategy> Strategies { get; init; } =
    [
        new NakedSubsetsStrategy(),
        new HiddenSinglesStrategy(),
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
            
            foreach (var strategy in Strategies)
            {
                MyGrid = strategy.Solve(MyGrid);
            }

            if (Equals(MyGrid.GetAllUnsolvedCells().Count(), previousGrid.GetAllUnsolvedCells().Count()))
            {
                unchangedIterations++;
            }
            else
            {
                unchangedIterations = 0;
            }
            
            if (unchangedIterations >= MaxUnchangedIterations)
            {
                break;
            }
        }

        return MyGrid;
    }
}