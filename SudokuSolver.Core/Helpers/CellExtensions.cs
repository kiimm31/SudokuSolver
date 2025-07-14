using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Helpers;

public static class CellExtensions
{
    public static List<Cell> Clone(this List<Cell> cells)
    {
        // Creates a deep copy of thelist of cells. 
        var serialize = JsonSerializer.Serialize(cells);

        return JsonSerializer.Deserialize<List<Cell>>(serialize)!;
    }
    

    public static List<int> Clone(this List<int> list)
    {
        return [..list];
    }
    
    public static Grid Clone(this Grid grid)
    {
        var allCells = new List<Cell>();
        
        // Iterate through all positions in the 9x9 grid
        for (int row = 1; row <= 9; row++)
        {
            for (int col = 1; col <= 9; col++)
            {
                var originalCell = grid.GetCell(row, col);
                allCells.Add(originalCell);
            }
        }
        
        // Clone the cells using the existing Clone method
        var clonedCells = allCells.Clone();
        
        // Create a new Grid with the cloned cells
        return new Grid(clonedCells);

    }
    
    public static bool IsSolved(this Grid grid)
    {
        var allCells = grid.GetAllCells();

        return allCells.All(cell => cell.IsSolved)
               && allCells.GroupBy(x => x.Row).All(y => y.ToList().HasAllValuesAndOnlyOnce())
               && allCells.GroupBy(x => x.Column).All(y => y.ToList().HasAllValuesAndOnlyOnce())
               && allCells.GroupBy(x => (x.Row - 1) / 3 * 3 + (x.Column - 1) / 3)
                   .All(y => y.ToList().HasAllValuesAndOnlyOnce());
    }
    
    public static bool IsRowSolved(this Grid grid, int row)
    {
        // Checks if a specific row is completely solved (all cells have values).
        var cells = grid.GetRow(row);
        return cells.All(cell => cell.IsSolved) && cells.HasAllValuesAndOnlyOnce();
    }
    
    public static bool IsColumnSolved(this Grid grid, int column)
    {
        // Checks if a specific column is completely solved (all cells have values).
        var cells = grid.GetColumn(column);
        return cells.All(cell => cell.IsSolved) && cells.HasAllValuesAndOnlyOnce();
    }
    
    public static bool IsBoxSolved(this Grid grid, int row, int column)
    {
        // Checks if the 3x3 box containing the specified cell is completely solved.
        var cells = grid.GetBox(row, column);
        return cells.All(cell => cell.IsSolved) && cells.HasAllValuesAndOnlyOnce();
    }

    public static bool HasAllValuesAndOnlyOnce(this List<Cell> referenceList)
    {
        // Checks if the reference list contains all values from 1 to 9 exactly once.
        var values = referenceList.Select(cell => cell.Value).Where(value => value != 0).ToList();
        return values.Count == 9 && values.Distinct().Count() == 9;
    }
}