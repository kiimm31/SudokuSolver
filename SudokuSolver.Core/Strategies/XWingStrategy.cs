using SudokuSolver.Core.Base;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Strategies;

/// <summary>
/// Strategy that finds X-Wing patterns - when a candidate appears in exactly two rows/columns
/// in exactly two columns/rows, it can be eliminated from other cells in those rows/columns
/// </summary>
public class XWingStrategy : BaseSolvingStrategy
{
    public override string Name => "X-Wing Strategy";
    public override int Priority => 4;
    
    protected override void ApplyToGroup(List<Cell> targetGroup)
    {
        // X-Wing strategy works across the entire grid, not just a single group
        // This method is not used for this strategy
    }
    
    public override Grid Apply(Grid grid)
    {
        // Check for X-Wing patterns in rows (candidate appears in exactly 2 columns)
        FindXWingInRows(grid);
        
        // Check for X-Wing patterns in columns (candidate appears in exactly 2 rows)
        FindXWingInColumns(grid);
        
        return grid;
    }
    
    private void FindXWingInRows(Grid grid)
    {
        foreach (var candidate in PossibleValues)
        {
            // For each candidate, check all possible row pairs
            for (int row1 = 1; row1 <= 8; row1++)
            {
                for (int row2 = row1 + 1; row2 <= 9; row2++)
                {
                    var row1Cells = grid.GetRow(row1).Where(c => c.GetPossibleValues().Contains(candidate)).ToList();
                    var row2Cells = grid.GetRow(row2).Where(c => c.GetPossibleValues().Contains(candidate)).ToList();
                    

                    
                    // Check if both rows have exactly 2 cells with this candidate
                    if (row1Cells.Count == 2 && row2Cells.Count == 2)
                    {
                        var row1Columns = row1Cells.Select(c => c.Column).OrderBy(c => c).ToList();
                        var row2Columns = row2Cells.Select(c => c.Column).OrderBy(c => c).ToList();
                        
                        // Check if the columns are the same (X-Wing pattern)
                        if (row1Columns.SequenceEqual(row2Columns))
                        {
                            // Eliminate the candidate from other cells in these columns
                            foreach (var col in row1Columns)
                            {
                                var columnCells = grid.GetColumn(col);
                                foreach (var cell in columnCells)
                                {
                                    if (cell.Row != row1 && cell.Row != row2 && cell.GetPossibleValues().Contains(candidate))
                                    {
                                        cell.EliminatePossibleValue(candidate);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    private void FindXWingInColumns(Grid grid)
    {
        foreach (var candidate in PossibleValues)
        {
            // For each candidate, check all possible column pairs
            for (int col1 = 1; col1 <= 8; col1++)
            {
                for (int col2 = col1 + 1; col2 <= 9; col2++)
                {
                    var col1Cells = grid.GetColumn(col1).Where(c => c.GetPossibleValues().Contains(candidate)).ToList();
                    var col2Cells = grid.GetColumn(col2).Where(c => c.GetPossibleValues().Contains(candidate)).ToList();
                    
                    // Check if both columns have exactly 2 cells with this candidate
                    if (col1Cells.Count == 2 && col2Cells.Count == 2)
                    {
                        var col1Rows = col1Cells.Select(c => c.Row).OrderBy(r => r).ToList();
                        var col2Rows = col2Cells.Select(c => c.Row).OrderBy(r => r).ToList();
                        
                        // Check if the rows are the same (X-Wing pattern)
                        if (col1Rows.SequenceEqual(col2Rows))
                        {
                            // Eliminate the candidate from other cells in these rows
                            foreach (var row in col1Rows)
                            {
                                var rowCells = grid.GetRow(row);
                                foreach (var cell in rowCells)
                                {
                                    if (cell.Column != col1 && cell.Column != col2 && cell.GetPossibleValues().Contains(candidate))
                                    {
                                        cell.EliminatePossibleValue(candidate);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
} 