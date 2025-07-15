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
        var clone = grid.GetAllCells().Clone();
        return new Grid(clone);

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
    public static bool IsPair(this Cell myCell, Cell otherCell)
    {
        if (myCell.IsSolved || otherCell.IsSolved) return false;
        var myCandidates = myCell.GetPossibleValues();
        var otherCandidates = otherCell.GetPossibleValues();

        // Check if both cells have exactly two candidates and they are the same
        return myCandidates.Count == 2 && otherCandidates.Count == 2 &&
               myCandidates.SequenceEqual(otherCandidates);
    }
}