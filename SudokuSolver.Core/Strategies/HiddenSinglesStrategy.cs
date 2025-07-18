using SudokuSolver.Core.Base;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Strategies;

/// <summary>
/// Strategy that finds hidden singles - cells that are the only ones in a group that can contain a specific value
/// </summary>
public class HiddenSinglesStrategy : BaseSolvingStrategy
{
    public override string Name => "Hidden Singles Strategy";
    public override int Priority => 1;
    
    protected override void ApplyToGroup(List<Cell> targetGroup)
    {
        foreach (var candidate in PossibleValues)
        {
            var candidateCells = targetGroup
                .Where(cell => cell.GetPossibleValues().Contains(candidate))
                .ToList();

            if (candidateCells.Count == 1)
            {
                var targetCell = candidateCells[0];
                
                // Only set the value if the cell is not already solved
                if (!targetCell.IsSolved)
                {
                    // Check if this value is already used in the target group
                    var usedValues = targetGroup.Where(c => c.IsSolved).Select(c => c.Value).ToList();
                    if (!usedValues.Contains(candidate))
                    {
                        targetCell.SetValue(candidate);
                    }
                }
            }
        }
    }
}