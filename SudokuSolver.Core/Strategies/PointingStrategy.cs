using System.Runtime.CompilerServices;
using SudokuSolver.Core.Base;
using SudokuSolver.Domain.Models;

[assembly: InternalsVisibleTo("SudokuSolver.UnitTest")]
namespace SudokuSolver.Core.Strategies;

/// <summary>
/// Strategy that finds pointing pairs/triples - when candidates in a box are restricted to one row or column
/// </summary>
public class PointingStrategy : BaseSolvingStrategy
{
    public override string Name => "Pointing Strategy";
    public override int Priority => 3;

    protected override void ApplyToGroup(List<Cell> targetGroup)
    {
        // This strategy works on boxes, so we need to override the Apply method
        // The ApplyToGroup method is not used for this strategy
    }
    
    public override Grid Apply(Grid grid)
    {
        // Iterate through each box in the grid
        var boxGroup = grid.GetAllUnsolvedCells().GroupBy(x => x.GetBoxIndex());
        foreach (var boxKvp in boxGroup)
        {
            ProcessBoxCandidates(grid, boxKvp);
        }

        return grid;
    }

    internal void ProcessBoxCandidates(Grid grid, IGrouping<int, Cell> boxKvp)
    {
        var boxNo = boxKvp.Key;
        var boxCells = boxKvp.ToList();

        var rowDictionary = new Dictionary<int, List<int>>(); //key candidate, value potential rows
        var columnDictionary = new Dictionary<int, List<int>>(); //key candidate, value potential columns
            
        foreach (var boxCell in boxCells)
        {
            ExtractCandidates(boxCell, boxCell.Row, rowDictionary);
            ExtractCandidates(boxCell, boxCell.Column, columnDictionary);
        }
         
        EliminateCandidateFromRow(grid, rowDictionary, boxNo);
        EliminateCandidateFromColumn(grid, columnDictionary, boxNo);
    }

    private static void EliminateCandidateFromColumn(Grid grid, Dictionary<int, List<int>> columnDictionary, int boxNo)
    {
        // Check columns for candidates that can be eliminated
        // is there a candidate that only appears in one column of the box?
        foreach (var (candidate, columnList) in columnDictionary)
        {
            if (columnList.Count != 1) 
                continue; 
            // Only one column contains this candidate
            var column = columnList[0];

            // Eliminate the candidate from all other cells in the column
            foreach (var cell in grid.GetColumn(column).Where(c =>
                         c.GetPossibleValues().Contains(candidate)
                         && c.GetBoxIndex() != boxNo))
            {
                cell.EliminatePossibleValue(candidate);
            }
        }
    }

    private static void EliminateCandidateFromRow(Grid grid, Dictionary<int, List<int>> rowDictionary, int boxNo)
    {
        // Check rows for candidates that can be eliminated
        // is there a candidate that only appears in one row of the box?
        foreach (var (candidate, rowList) in rowDictionary)
        {
            if (rowList.Count != 1) 
                continue; 
            // Only one row contains this candidate
            var rowIndex = rowList[0];

            // Eliminate the candidate from all other cells in the row
            foreach (var cell in grid.GetRow(rowIndex).Where(c =>
                         c.GetPossibleValues().Contains(candidate) && c.GetBoxIndex() != boxNo))
            {
                cell.EliminatePossibleValue(candidate);
            }
        }
    }

    private static void ExtractCandidates(Cell referenceCell, int groupKey, Dictionary<int, List<int>> groupDictionary)
    {
        foreach (var candidate in referenceCell.GetPossibleValues())
        {
            if (groupDictionary.ContainsKey(candidate))
            {
                if (!groupDictionary[candidate].Contains(groupKey))
                {
                    groupDictionary[candidate].Add(groupKey);
                }
            }
            else
            {
                groupDictionary.Add(candidate, [groupKey]);
            }
        }
    }
}