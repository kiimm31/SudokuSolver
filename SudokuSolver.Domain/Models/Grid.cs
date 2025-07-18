using System.Collections;
using System.Text;

namespace SudokuSolver.Domain.Models;

public class Grid(List<Cell> cells)
{
    // Represents a Sudoku grid, which is a 9x9 matrix of cells.
    private List<Cell> Cells { get; set; } = cells;

    public Cell? GetCell(int row, int column)
    {
        // Retrieves a cell from the grid based on its row and column indices.
        return Cells.SingleOrDefault(c => c.Row == row && c.Column == column);
    }

    public void SetCell(int row, int column, int value)
    {
        // Sets the value of a specific cell in the grid.
        var cell = GetCell(row, column);
        if (cell.Value != 0)
            cell.Value = value;
        else
            throw new InvalidOperationException("Cell is already set.");
    }

    public List<Cell> GetRow(int row)
    {
        // Retrieves all cells in a specific row.
        return Cells.Where(c => c.Row == row)
            .OrderBy(x => x.Column)
            .ToList();
    }

    public List<Cell> GetColumn(int column)
    {
        // Retrieves all cells in a specific column.
        return Cells.Where(c => c.Column == column)
            .OrderBy(x => x.Row)
            .ToList();
    }

    public List<Cell> GetBox(int row, int column)
    {
        // Retrieves all cells in the 3x3 box that contains the specified cell.
        // row and column are 1-based cell coordinates (1-9)

        var cells = new List<Cell>();

        // Determine which box the cell belongs to (convert to 0-based for calculation)
        var boxRow = (row - 1) / 3;
        var boxColumn = (column - 1) / 3;

        // Calculate the starting row and column for the box (convert back to 1-based)
        var startRow = boxRow * 3 + 1;
        var startColumn = boxColumn * 3 + 1;

        // Iterate through the 3x3 box
        for (var r = startRow; r < startRow + 3; r++)
        {
            for (var c = startColumn; c < startColumn + 3; c++)
            {
                var cell = GetCell(r, c);
                if (cell is not null)
                {
                    cells.Add(cell);
                }
            }
        }

        return cells;
    }

    public IEnumerable<Cell> GetAllUnsolvedCells()
    {
        // Retrieves all unsolved cells in the grid.
        return Cells.Where(c => !c.IsSolved);
    }

    public List<Cell> GetAllCells()
    {
        // Retrieves all cells in the grid.
        return Cells;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (int row = 1; row <= 9; row++)
        {
            // Add horizontal separator before rows 4 and 7
            if (row == 4 || row == 7)
            {
                sb.AppendLine("------+-------+------");
            }

            for (int col = 1; col <= 9; col++)
            {
                var cell = GetCell(row, col);
                var value = cell.Value == 0 ? "." : cell.Value.ToString();
                sb.Append(value);

                // Add vertical separator after columns 3 and 6
                if (col % 3 == 0)
                {
                    sb.Append(" | ");
                }
                else if (col < 9) // Add space between numbers (except after the last column)
                {
                    sb.Append(" ");
                }
            }

            sb.AppendLine(); // End the row
        }

        return sb.ToString();
    }

    public override bool Equals(object? obj)
    {
        var referenceGrid = (Grid)obj!;

        var me = GetAllCells().ToList();

        foreach (var myCell in me)
        {
            var referenceCell = referenceGrid.GetCell(myCell.Row, myCell.Column)!;

            if (myCell.IsConfirmed && referenceCell.IsConfirmed)
            {
                continue;
            }

            // Check if the cell values are equal
            if (myCell.Value != referenceCell.Value)
            {
                return false;
            }

            // Check if the possible values are equal
            if (myCell.GetPossibleValues().Count != referenceCell.GetPossibleValues().Count)
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerable<Cell> GetSolvedCells()
    {
        return Cells.Where(x => x.IsSolved);
    }
    
    /// <summary>
    /// Checks if the grid is completely solved
    /// </summary>
    /// <returns>True if all cells are solved, false otherwise</returns>
    public bool IsSolved()
    {
        return Cells.All(c => c.IsSolved);
    }
    
    /// <summary>
    /// Creates a deep copy of the grid
    /// </summary>
    /// <returns>A new grid with the same cell values and possible values</returns>
    public Grid Clone()
    {
        var clonedCells = Cells.Select(c => c.Clone()).ToList();
        return new Grid(clonedCells);
    }
}