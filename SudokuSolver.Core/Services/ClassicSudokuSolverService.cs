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
    public override List<Constraint> Constrains { get; init; } =
    [
        new RowConstraint(),
        new ColumnConstraint(),
        new BoxConstraint()
    ];
    
    private int MaxUnchangedIterations { get; init; } = 5;
    
    public override List<Strategy> Strategies { get; init; } =
    [
        new NakedSubsetsStrategy(),
        new HiddenSinglesStrategy(),
        new PointingStrategy(),
    ];

    public override Grid Solve()
    {
        if (MyGrid is null)
        {
            throw new ArgumentNullException(nameof(grid));
        }

        var unchangedIterations = 0;

        try
        {
            while (!MyGrid.IsSolved())
            {
                var previousGrid = MyGrid.Clone();
                var previousUnsolvedCount = previousGrid.GetAllUnsolvedCells().Count();

                // Apply constraints to eliminate impossible values
                foreach (var constrain in Constrains)
                {
                    foreach (var referenceCell in MyGrid.GetAllUnsolvedCells())
                    {
                        MyGrid = constrain.TrySolve(MyGrid, referenceCell.Row, referenceCell.Column);
                    }
                }
                
                // Validate the grid after constraint application
                try
                {
                    MyGrid.ObeysAllConstraint();
                }
                catch (System.Data.InvalidConstraintException ex)
                {
                    Console.WriteLine($"Constraint violation detected: {ex.Message}");
                    break; // Stop solving if we have a constraint violation
                }
            
                // Apply advanced solving strategies one by one
                foreach (var strategy in Strategies)
                {
                    var strategyPreviousGrid = MyGrid.Clone();
                    MyGrid = strategy.Solve(MyGrid);
                    
                    // Validate after each strategy
                    try
                    {
                        MyGrid.ObeysAllConstraint();
                    }
                    catch (System.Data.InvalidConstraintException ex)
                    {
                        // Revert to previous state and skip this strategy
                        MyGrid = strategyPreviousGrid;
                        continue;
                    }
                }

                var currentUnsolvedCount = MyGrid.GetAllUnsolvedCells().Count();
                
                if (currentUnsolvedCount == previousUnsolvedCount)
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
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during solving: {e.Message}");
            Console.WriteLine($"Grid state:\n{MyGrid}");
            throw;
        }

        return MyGrid;
    }
}