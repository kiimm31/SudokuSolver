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
                    Value = gridRaw[row, col]
                };
                cells.Add(cell);
            }
        }

        return new Grid(cells);
    }
}