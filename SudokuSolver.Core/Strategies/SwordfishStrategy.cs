using SudokuSolver.Core.Base;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Strategies;

/// <summary>
/// Strategy that finds Swordfish patterns - when a candidate appears in exactly three rows/columns
/// in exactly three columns/rows, it can be eliminated from other cells in those rows/columns
/// </summary>
public class SwordfishStrategy : BaseSolvingStrategy
{
    public override string Name => "Swordfish Strategy";
    public override int Priority => 5;
    
    protected override void ApplyToGroup(List<Cell> targetGroup)
    {
        // Swordfish strategy works across the entire grid, not just a single group
        // This method is not used for this strategy
    }
    
    public override Grid Apply(Grid grid)
    {
        // Check for Swordfish patterns in rows (the candidate appears in exactly 3 columns)
        FindSwordfishInRows(grid);
        
        // Check for Swordfish patterns in columns (the candidate appears in exactly 3 rows)
        FindSwordfishInColumns(grid);
        
        return grid;
    }
    
    private void FindSwordfishInRows(Grid grid)
    {
        foreach (var candidate in PossibleValues)
        {
            // For each candidate, check all possible row triplets
            for (var row1 = 1; row1 <= 7; row1++)
            {
                for (var row2 = row1 + 1; row2 <= 8; row2++)
                {
                    for (var row3 = row2 + 1; row3 <= 9; row3++)
                    {
                        var row1Cells = grid.GetRow(row1).Where(c => c.GetPossibleValues().Contains(candidate)).ToList();
                        var row2Cells = grid.GetRow(row2).Where(c => c.GetPossibleValues().Contains(candidate)).ToList();
                        var row3Cells = grid.GetRow(row3).Where(c => c.GetPossibleValues().Contains(candidate)).ToList();
                        
                        // Check if all three rows have exactly 3 cells with this candidate
                        if (row1Cells.Count == 3 && row2Cells.Count == 3 && row3Cells.Count == 3)
                        {
                            var row1Columns = row1Cells.Select(c => c.Column).OrderBy(c => c).ToList();
                            var row2Columns = row2Cells.Select(c => c.Column).OrderBy(c => c).ToList();
                            var row3Columns = row3Cells.Select(c => c.Column).OrderBy(c => c).ToList();
                            

                            
                            // Check if the columns are the same (Swordfish pattern)
                            if (row1Columns.SequenceEqual(row2Columns) && row2Columns.SequenceEqual(row3Columns))
                            {
                                // Eliminate the candidate from other cells in these columns
                                foreach (var cell in row1Columns.Select(grid.GetColumn)
                                             .SelectMany(columnCells => columnCells,
                                                 (columnCells, cell) => new { columnCells, cell })
                                             .Where(c =>
                                                 c.cell.Row != row1 && c.cell.Row != row2 && c.cell.Row != row3 &&
                                                 c.cell.GetPossibleValues().Contains(candidate))
                                             .Select(@t => @t.cell))
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
    
    private void FindSwordfishInColumns(Grid grid)
    {
        foreach (var candidate in PossibleValues)
        {
            // For each candidate, check all possible column triplets
            for (var col1 = 1; col1 <= 7; col1++)
            {
                for (var col2 = col1 + 1; col2 <= 8; col2++)
                {
                    for (var col3 = col2 + 1; col3 <= 9; col3++)
                    {
                        var col1Cells = grid.GetColumn(col1).Where(c => c.GetPossibleValues().Contains(candidate)).ToList();
                        var col2Cells = grid.GetColumn(col2).Where(c => c.GetPossibleValues().Contains(candidate)).ToList();
                        var col3Cells = grid.GetColumn(col3).Where(c => c.GetPossibleValues().Contains(candidate)).ToList();
                        
                        // Check if all three columns have exactly 3 cells with this candidate
                        if (col1Cells.Count == 3 && col2Cells.Count == 3 && col3Cells.Count == 3)
                        {
                            var col1Rows = col1Cells.Select(c => c.Row).OrderBy(r => r).ToList();
                            var col2Rows = col2Cells.Select(c => c.Row).OrderBy(r => r).ToList();
                            var col3Rows = col3Cells.Select(c => c.Row).OrderBy(r => r).ToList();
                            
                            // Check if the rows are the same (Swordfish pattern)
                            if (col1Rows.SequenceEqual(col2Rows) && col2Rows.SequenceEqual(col3Rows))
                            {
                                // Eliminate the candidate from other cells in these rows
                                foreach (var cell in col1Rows.Select(grid.GetRow)
                                             .SelectMany(rowCells => rowCells,
                                                 (rowCells, cell) => new { rowCells, cell })
                                             .Where(c =>
                                                 c.cell.Column != col1 && c.cell.Column != col2 &&
                                                 c.cell.Column != col3 &&
                                                 c.cell.GetPossibleValues().Contains(candidate))
                                             .Select(@t => @t.cell))
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