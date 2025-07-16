using System.Runtime.CompilerServices;
using SudokuSolver.Core.Interface;
using SudokuSolver.Domain.Models;

[assembly: InternalsVisibleTo("SudokuSolver.UnitTest")]
namespace SudokuSolver.Core.Strategies;

public class PointingStrategy : Strategy
{
    public override string Name => "Pointing Strategy";

    protected override void DoWork(List<Cell> targetGroup)
    {
        // This method is not used in PointingStrategy, as the logic is handled in Solve method
        // It can be left empty or throw an exception if called
        throw new NotImplementedException("PointingStrategy does not use DoWork method.");
    }

    public override Grid Solve(Grid grid)
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
        // Check rows for candidates that can be eliminated
        // is there a candidate that only appears in one row of the box?
        foreach (var (candidate, columnList) in columnDictionary)
        {
            if (columnList.Count != 1) 
                continue; 
            // Only one row contains this candidate
            var column = columnList[0];

            // Eliminate the candidate from all other cells in the row
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