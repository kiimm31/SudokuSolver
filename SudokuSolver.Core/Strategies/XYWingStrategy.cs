using SudokuSolver.Core.Base;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Strategies;

/// <summary>
/// Strategy that finds XY-Wing patterns - when a pivot cell has candidates XY and two pincer cells
/// have candidates XZ and YZ respectively, the common value Z can be eliminated from cells that see both pincers
/// </summary>
public class XYWingStrategy : BaseSolvingStrategy
{
    public override string Name => "XY-Wing Strategy";
    public override int Priority => 6;
    
    protected override void ApplyToGroup(List<Cell> targetGroup)
    {
        // XY-Wing strategy works across the entire grid, not just a single group
        // This method is not used for this strategy
    }
    
    public override Grid Apply(Grid grid)
    {
        var unsolvedCells = grid.GetAllUnsolvedCells().ToList();
        
        // Find potential pivot cells (cells with exactly 2 candidates)
        var potentialPivots = unsolvedCells.Where(c => c.GetPossibleValues().Count == 2).ToList();
        
        foreach (var pivot in potentialPivots)
        {
            FindXYWingPattern(grid, pivot);
        }
        
        return grid;
    }
    
    private void FindXYWingPattern(Grid grid, Cell pivot)
    {
        var pivotCandidates = pivot.GetPossibleValues();
        if (pivotCandidates.Count != 2)
            return;
        
        var x = pivotCandidates[0];
        var y = pivotCandidates[1];
        
        // Find cells that see the pivot and have exactly 2 candidates
        var cellsThatSeePivot = GetCellsThatSeeCell(grid, pivot)
            .Where(c => c.GetPossibleValues().Count == 2 && !c.IsSolved)
            .ToList();
        
        // Look for pincer cells
        for (int i = 0; i < cellsThatSeePivot.Count; i++)
        {
            for (int j = i + 1; j < cellsThatSeePivot.Count; j++)
            {
                var pincer1 = cellsThatSeePivot[i];
                var pincer2 = cellsThatSeePivot[j];
                
                var pincer1Candidates = pincer1.GetPossibleValues();
                var pincer2Candidates = pincer2.GetPossibleValues();
                
                // Check if pincer1 has X and another value (XZ)
                if (pincer1Candidates.Contains(x) && pincer1Candidates.Count == 2)
                {
                    var z1 = pincer1Candidates.First(c => c != x);
                    
                    // Check if pincer2 has Y and the same Z value (YZ)
                    if (pincer2Candidates.Contains(y) && pincer2Candidates.Contains(z1) && pincer2Candidates.Count == 2)
                    {
                        // Found XY-Wing pattern! Eliminate Z from cells that see both pincers
                        EliminateZFromCommonCells(grid, pincer1, pincer2, z1);
                    }
                }
                
                // Check if pincer1 has Y and another value (YZ)
                if (pincer1Candidates.Contains(y) && pincer1Candidates.Count == 2)
                {
                    var z2 = pincer1Candidates.First(c => c != y);
                    
                    // Check if pincer2 has X and the same Z value (XZ)
                    if (pincer2Candidates.Contains(x) && pincer2Candidates.Contains(z2) && pincer2Candidates.Count == 2)
                    {
                        // Found XY-Wing pattern! Eliminate Z from cells that see both pincers
                        EliminateZFromCommonCells(grid, pincer1, pincer2, z2);
                    }
                }
            }
        }
    }
    
    private void EliminateZFromCommonCells(Grid grid, Cell pincer1, Cell pincer2, int z)
    {
        // Find cells that see both pincers
        var cellsThatSeePincer1 = GetCellsThatSeeCell(grid, pincer1);
        var cellsThatSeePincer2 = GetCellsThatSeeCell(grid, pincer2);
        
        var commonCells = cellsThatSeePincer1.Intersect(cellsThatSeePincer2)
            .Where(c => c.GetPossibleValues().Contains(z) && !c.IsSolved)
            .ToList();
        
        foreach (var cell in commonCells)
        {
            cell.EliminatePossibleValue(z);
        }
    }
    
    private IEnumerable<Cell> GetCellsThatSeeCell(Grid grid, Cell targetCell)
    {
        var cells = new List<Cell>();
        
        // Add cells in the same row
        cells.AddRange(grid.GetRow(targetCell.Row));
        
        // Add cells in the same column
        cells.AddRange(grid.GetColumn(targetCell.Column));
        
        // Add cells in the same box
        cells.AddRange(grid.GetBox(targetCell.Row, targetCell.Column));
        
        // Remove duplicates and the target cell itself
        return cells.Distinct().Where(c => c != targetCell);
    }
} 