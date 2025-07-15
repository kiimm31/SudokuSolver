using SudokuSolver.Core.Interface;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Strategies;

public class HiddenSinglesStrategy : Strategy
{
    public override string Name => "Hidden Singles Strategy";
    protected override void DoWork(List<Cell> targetGroup)
    {
        foreach (var candidates in PossibleValues)    
        {
            var candidateCells = targetGroup
                .Where(cell => cell.GetPossibleValues().Contains(candidates))
                .ToList();

            if (candidateCells.Count == 1)
            {
                var targetCell = candidateCells[0];
                targetCell.SetValue(candidates);
            }
        }
    }
}