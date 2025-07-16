using SudokuSolver.Domain.Models;

namespace SudokuSolver.UnitTest.Base;

public class BaseTest
{
    protected Grid CreateFullGrid()
    {
        var cells = new List<Cell>();
        for (int row = 1; row <= 9; row++)
        {
            for (int column = 1; column <= 9; column++)
            {
                cells.Add(new Cell { Row = row, Column = column, Value = 0 });
            }
        }
        return new Grid(cells);
    }
    
    protected Grid GenerateGrid(int[,] gridRaw)
    {
        var cells = new List<Cell>();
        // Convert to Cell objects (using 1-based indexing as shown in your tests)
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell = new Cell 
                { 
                    Row = row + 1,      // Convert to 1-based indexing
                    Column = col + 1,   // Convert to 1-based indexing
                };

                if (gridRaw[row, col] == 0)
                {
                    // If the cell is empty, we can leave it as is
                    // or initialize it with possible values if needed
                    // cell.PossibleValues = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                }
                else
                {
                    cell.SetValue(gridRaw[row, col]);
                }
               
                cells.Add(cell);
            }
        }

        return new Grid(cells);
    }
    
    private static readonly int[] _allPossibleValues = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    protected Cell GenerateCellWithPossibleValues(int row, int column, int[] possibleValues = null)
    {
        var myCell = new Cell { Row = row, Column = column};

        if (!(possibleValues?.Any() ?? false)) 
            return myCell;
        foreach (var candidate in _allPossibleValues.Where(x => !possibleValues.Contains(x)))
        {
            myCell.EliminatePossibleValue(candidate);
        }

        return myCell;
    }
    
    protected Cell GenerateCellWithValue(int row, int column, int value)
    {
        var myCell = new Cell { Row = row, Column = column };
        myCell.SetValue(value);
        return myCell;
    }
}