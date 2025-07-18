using SudokuSolver.Core.Constrains;
using SudokuSolver.Core.Helpers;
using SudokuSolver.Core.Interface;
using SudokuSolver.Core.Strategies;
using SudokuSolver.Domain;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Services;

public class ClassicSudokuSolverService(Grid grid) : AbstractSudokuSolverService
{
    private int _unchangedIterations = 0;
    private static int MaxUnchangedIterations => 5;
    public override Grid MyGrid { get; set; } = grid;
    public override List<Constraint> Constrains { get; init; } =
    [
        new RowConstraint(),
        new ColumnConstraint(),
        new BoxConstraint()
    ];
    
    public override List<Strategy> Strategies { get; init; } =
    [
        new NakedSubsetsStrategy(),
        new HiddenSinglesStrategy(),
        new PointingStrategy(),
    ];

    public override Grid Solve()
    {
        ValidateInput();

        try
        {
            while (!MyGrid.IsSolved())
            {
                var previousUnsolvedCount = MyGrid.GetAllUnsolvedCells().Count();

                ApplyConstraints();
                
                if (HasConstraintViolation())
                {
                    break;
                }
            
                ApplyStrategies();

                _unchangedIterations = UpdateProgressTracking(previousUnsolvedCount, _unchangedIterations);
            
                if (ShouldStopSolving(_unchangedIterations))
                {
                    break;
                }
            }
        }
        catch (Exception e)
        {
            HandleSolvingError(e);
        }

        return MyGrid;
    }

    private void ValidateInput()
    {
        if (MyGrid is null)
        {
            throw new ArgumentNullException(nameof(grid));
        }
        
        if (!MyGrid.GetAllUnsolvedCells().Any())
        {
            throw new InvalidOperationException("The grid is already solved.");
        }
        
        if (MyGrid.GetAllUnsolvedCells().Count() > 81)
        {
            throw new InvalidOperationException("The grid has more than 81 unsolved cells, which is invalid.");
        }

        _unchangedIterations = 0;
    }

    private void ApplyConstraints()
    {
        foreach (var constrain in Constrains)
        {
            foreach (var referenceCell in MyGrid.GetAllUnsolvedCells())
            {
                MyGrid = constrain.TrySolve(MyGrid, referenceCell.Row, referenceCell.Column);
            }
        }
    }

    private bool HasConstraintViolation()
    {
        try
        {
            MyGrid.ObeysAllConstraint();
            return false;
        }
        catch (System.Data.InvalidConstraintException ex)
        {
            Console.WriteLine($"Constraint violation detected: {ex.Message}");
            return true;
        }
    }

    private void ApplyStrategies()
    {
        foreach (var strategy in Strategies)
        {
            var strategyPreviousGrid = MyGrid.Clone();
            MyGrid = strategy.Solve(MyGrid);
            
            if (HasConstraintViolation())
            {
                // Revert to previous state and skip this strategy
                MyGrid = strategyPreviousGrid;
            }
        }
    }

    private int UpdateProgressTracking(int previousUnsolvedCount, int unchangedIterations)
    {
        var currentUnsolvedCount = MyGrid.GetAllUnsolvedCells().Count();
        
        if (currentUnsolvedCount == previousUnsolvedCount)
        {
            return unchangedIterations + 1;
        }
        
        return 0;
    }

    private bool ShouldStopSolving(int unchangedIterations)
    {
        return unchangedIterations >= MaxUnchangedIterations;
    }

    private void HandleSolvingError(Exception e)
    {
        Console.WriteLine($"Error during solving: {e.Message}");
        Console.WriteLine($"Grid state:\n{MyGrid}");
        throw new InvalidOperationException($"Error during solving: {e.Message}", e);
    }
}