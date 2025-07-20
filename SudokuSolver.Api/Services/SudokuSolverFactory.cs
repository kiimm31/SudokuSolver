using SudokuSolver.Core.Constraints;
using SudokuSolver.Core.Interfaces;
using SudokuSolver.Core.Services;
using SudokuSolver.Core.Strategies;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Api.Services;

/// <summary>
/// Factory for creating Sudoku solver instances
/// </summary>
public interface ISudokuSolverFactory
{
    /// <summary>
    /// Creates a new ClassicSudokuSolver instance
    /// </summary>
    /// <returns>A configured ClassicSudokuSolver instance</returns>
    ISudokuSolver CreateSolver();
}

/// <summary>
/// Implementation of SudokuSolverFactory
/// </summary>
public class SudokuSolverFactory : ISudokuSolverFactory
{
    public ISudokuSolver CreateSolver()
    {
        // Create a default empty grid
        var cells = new List<Cell>();
        for (int row = 1; row <= 9; row++)
        {
            for (int col = 1; col <= 9; col++)
            {
                cells.Add(new Cell { Row = row, Column = col });
            }
        }
        var defaultGrid = new Grid(cells);

        // Create constraints
        var constraints = new List<IConstraint>
        {
            new RowConstraint(),
            new ColumnConstraint(),
            new BoxConstraint()
        };

        // Create strategies
        var strategies = new List<ISolvingStrategy>
        {
            new HiddenSinglesStrategy(),
            new NakedSubsetsStrategy(),
            new PointingStrategy(),
            new XWingStrategy(),
            new SwordfishStrategy(),
            new XYWingStrategy()
        };

        return new ClassicSudokuSolver(defaultGrid, constraints, strategies);
    }
} 