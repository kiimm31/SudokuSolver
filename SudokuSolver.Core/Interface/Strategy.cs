using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Interface;

public abstract class Strategy
{
    protected static readonly List<int> PossibleValues = [1, 2, 3, 4, 5, 6, 7, 8, 9];

    public abstract string Name { get; }
    public virtual Grid Solve(Grid grid)
    {
        var unsolvedCells = grid.GetAllUnsolvedCells();
        foreach (var rowGroup in unsolvedCells.GroupBy(r => r.Row))
        {
            DoWork(rowGroup.ToList());
        }

        foreach (var colGroup in unsolvedCells.GroupBy(c => c.Column))
        {
            DoWork(colGroup.ToList());
        }

        foreach (var boxGroup in unsolvedCells.GroupBy(c => c.GetBoxIndex()))
        {
            DoWork(boxGroup.ToList());
        }

        return grid;
    }

    /// <summary>
    /// will process for each group of cells (row, column, box)
    /// </summary>
    /// <param name="targetGroup"></param>
    protected abstract void DoWork(List<Cell> targetGroup);
}