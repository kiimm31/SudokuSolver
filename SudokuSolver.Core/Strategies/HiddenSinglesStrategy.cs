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
                
                // Only set the value if the cell is not already solved
                if (!targetCell.IsSolved)
                {
                    // Check if this value is already used in the target group
                    var usedValues = targetGroup.Where(c => c.IsSolved).Select(c => c.Value).ToList();
                    if (!usedValues.Contains(candidates))
                    {
                        targetCell.SetValue(candidates);
                    }
                }
            }
        }
    }
}