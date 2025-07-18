using FluentAssertions;
using SudokuSolver.Core.Strategies;
using SudokuSolver.Domain.Models;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest.Strategies;

public class XWingStrategyTest : BaseTest
{
    [Test]
    public void XWing_Should_Eliminate_Candidates_From_Columns()
    {
        // Arrange: Create a grid with X-Wing pattern in columns 1 and 3
        var cells = new List<Cell>();
        
        // Fill the grid with cells that have all possible values
        for (int row = 1; row <= 9; row++)
        {
            for (int col = 1; col <= 9; col++)
            {
                cells.Add(GenerateCellWithPossibleValues(row, col, [1, 2, 3, 4, 5, 6, 7, 8, 9]));
            }
        }
        
        var grid = new Grid(cells);
        
        // Set up X-Wing pattern: value 5 appears only in rows 1,3 and columns 1,3
        // Row 1: value 5 only in columns 1 and 3
        grid.GetCell(1, 1)!.EliminatePossibleValue(2);
        grid.GetCell(1, 1)!.EliminatePossibleValue(3);
        grid.GetCell(1, 1)!.EliminatePossibleValue(4);
        grid.GetCell(1, 1)!.EliminatePossibleValue(6);
        grid.GetCell(1, 1)!.EliminatePossibleValue(7);
        grid.GetCell(1, 1)!.EliminatePossibleValue(8);
        grid.GetCell(1, 1)!.EliminatePossibleValue(9);
        
        grid.GetCell(1, 3)!.EliminatePossibleValue(2);
        grid.GetCell(1, 3)!.EliminatePossibleValue(3);
        grid.GetCell(1, 3)!.EliminatePossibleValue(4);
        grid.GetCell(1, 3)!.EliminatePossibleValue(6);
        grid.GetCell(1, 3)!.EliminatePossibleValue(7);
        grid.GetCell(1, 3)!.EliminatePossibleValue(8);
        grid.GetCell(1, 3)!.EliminatePossibleValue(9);
        
        // Row 3: value 5 only in columns 1 and 3
        grid.GetCell(3, 1)!.EliminatePossibleValue(2);
        grid.GetCell(3, 1)!.EliminatePossibleValue(3);
        grid.GetCell(3, 1)!.EliminatePossibleValue(4);
        grid.GetCell(3, 1)!.EliminatePossibleValue(6);
        grid.GetCell(3, 1)!.EliminatePossibleValue(7);
        grid.GetCell(3, 1)!.EliminatePossibleValue(8);
        grid.GetCell(3, 1)!.EliminatePossibleValue(9);
        
        grid.GetCell(3, 3)!.EliminatePossibleValue(2);
        grid.GetCell(3, 3)!.EliminatePossibleValue(3);
        grid.GetCell(3, 3)!.EliminatePossibleValue(4);
        grid.GetCell(3, 3)!.EliminatePossibleValue(6);
        grid.GetCell(3, 3)!.EliminatePossibleValue(7);
        grid.GetCell(3, 3)!.EliminatePossibleValue(8);
        grid.GetCell(3, 3)!.EliminatePossibleValue(9);
        
        // Remove value 5 from other cells in rows 1 and 3 (except columns 1 and 3)
        for (int col = 1; col <= 9; col++)
        {
            if (col != 1 && col != 3)
            {
                grid.GetCell(1, col)!.EliminatePossibleValue(5);
                grid.GetCell(3, col)!.EliminatePossibleValue(5);
            }
        }
        
        // Remove value 5 from other cells in columns 1 and 3 (except rows 1 and 3)
        for (int row = 1; row <= 9; row++)
        {
            if (row != 1 && row != 3)
            {
                grid.GetCell(row, 1)!.EliminatePossibleValue(5);
                grid.GetCell(row, 3)!.EliminatePossibleValue(5);
            }
        }
        
        // Add a cell in column 1 that has value 5 as a candidate (should be eliminated)
        grid.GetCell(2, 1)!.EliminatePossibleValue(2);
        grid.GetCell(2, 1)!.EliminatePossibleValue(3);
        grid.GetCell(2, 1)!.EliminatePossibleValue(4);
        grid.GetCell(2, 1)!.EliminatePossibleValue(6);
        grid.GetCell(2, 1)!.EliminatePossibleValue(7);
        grid.GetCell(2, 1)!.EliminatePossibleValue(8);
        grid.GetCell(2, 1)!.EliminatePossibleValue(9);
        
        // Act: Apply X-Wing strategy
        var strategy = new XWingStrategy();
        var result = strategy.Apply(grid);
        

        
        // Assert: Value 5 should be eliminated from cell (2,1)
        var targetCell = result.GetCell(2, 1)!;
        targetCell.GetPossibleValues().Should().NotContain(5);
    }
    
    [Test]
    public void XWing_Should_Eliminate_Candidates_From_Rows()
    {
        // Arrange: Create a grid with X-Wing pattern in rows 1 and 3
        var cells = new List<Cell>();
        
        // Fill the grid with cells that have all possible values
        for (int row = 1; row <= 9; row++)
        {
            for (int col = 1; col <= 9; col++)
            {
                cells.Add(GenerateCellWithPossibleValues(row, col, [1, 2, 3, 4, 5, 6, 7, 8, 9]));
            }
        }
        
        var grid = new Grid(cells);
        
        // Set up X-Wing pattern: value 7 appears only in columns 1,3 and rows 1,3
        // Column 1: value 7 only in rows 1 and 3
        grid.GetCell(1, 1)!.EliminatePossibleValue(2);
        grid.GetCell(1, 1)!.EliminatePossibleValue(3);
        grid.GetCell(1, 1)!.EliminatePossibleValue(4);
        grid.GetCell(1, 1)!.EliminatePossibleValue(5);
        grid.GetCell(1, 1)!.EliminatePossibleValue(6);
        grid.GetCell(1, 1)!.EliminatePossibleValue(8);
        grid.GetCell(1, 1)!.EliminatePossibleValue(9);
        
        grid.GetCell(3, 1)!.EliminatePossibleValue(2);
        grid.GetCell(3, 1)!.EliminatePossibleValue(3);
        grid.GetCell(3, 1)!.EliminatePossibleValue(4);
        grid.GetCell(3, 1)!.EliminatePossibleValue(5);
        grid.GetCell(3, 1)!.EliminatePossibleValue(6);
        grid.GetCell(3, 1)!.EliminatePossibleValue(8);
        grid.GetCell(3, 1)!.EliminatePossibleValue(9);
        
        // Column 3: value 7 only in rows 1 and 3
        grid.GetCell(1, 3)!.EliminatePossibleValue(2);
        grid.GetCell(1, 3)!.EliminatePossibleValue(3);
        grid.GetCell(1, 3)!.EliminatePossibleValue(4);
        grid.GetCell(1, 3)!.EliminatePossibleValue(5);
        grid.GetCell(1, 3)!.EliminatePossibleValue(6);
        grid.GetCell(1, 3)!.EliminatePossibleValue(8);
        grid.GetCell(1, 3)!.EliminatePossibleValue(9);
        
        grid.GetCell(3, 3)!.EliminatePossibleValue(2);
        grid.GetCell(3, 3)!.EliminatePossibleValue(3);
        grid.GetCell(3, 3)!.EliminatePossibleValue(4);
        grid.GetCell(3, 3)!.EliminatePossibleValue(5);
        grid.GetCell(3, 3)!.EliminatePossibleValue(6);
        grid.GetCell(3, 3)!.EliminatePossibleValue(8);
        grid.GetCell(3, 3)!.EliminatePossibleValue(9);
        
        // Remove value 7 from other cells in columns 1 and 3 (except rows 1 and 3)
        for (int row = 1; row <= 9; row++)
        {
            if (row != 1 && row != 3)
            {
                grid.GetCell(row, 1)!.EliminatePossibleValue(7);
                grid.GetCell(row, 3)!.EliminatePossibleValue(7);
            }
        }
        
        // Remove value 7 from other cells in rows 1 and 3 (except columns 1 and 3)
        for (int col = 1; col <= 9; col++)
        {
            if (col != 1 && col != 3)
            {
                grid.GetCell(1, col)!.EliminatePossibleValue(7);
                grid.GetCell(3, col)!.EliminatePossibleValue(7);
            }
        }
        
        // Add a cell in row 2 that has value 7 as a candidate (should be eliminated)
        grid.GetCell(2, 1)!.EliminatePossibleValue(2);
        grid.GetCell(2, 1)!.EliminatePossibleValue(3);
        grid.GetCell(2, 1)!.EliminatePossibleValue(4);
        grid.GetCell(2, 1)!.EliminatePossibleValue(5);
        grid.GetCell(2, 1)!.EliminatePossibleValue(6);
        grid.GetCell(2, 1)!.EliminatePossibleValue(8);
        grid.GetCell(2, 1)!.EliminatePossibleValue(9);
        
        // Act: Apply X-Wing strategy
        var strategy = new XWingStrategy();
        var result = strategy.Apply(grid);
        
        // Assert: Value 7 should be eliminated from cell (2,1)
        var targetCell = result.GetCell(2, 1)!;
        targetCell.GetPossibleValues().Should().NotContain(7);
    }
    
    [Test]
    public void XWing_Should_Not_Eliminate_When_No_XWing_Pattern_Exists()
    {
        // Arrange: Create a grid without X-Wing pattern
        var cells = new List<Cell>();
        
        // Fill the grid with cells that have all possible values
        for (int row = 1; row <= 9; row++)
        {
            for (int col = 1; col <= 9; col++)
            {
                cells.Add(GenerateCellWithPossibleValues(row, col, [1, 2, 3, 4, 5, 6, 7, 8, 9]));
            }
        }
        
        var grid = new Grid(cells);
        
        // Store original possible values for cell (2,2)
        var originalPossibleValues = grid.GetCell(2, 2)!.GetPossibleValues().ToList();
        
        // Act: Apply X-Wing strategy
        var strategy = new XWingStrategy();
        var result = strategy.Apply(grid);
        
        // Assert: No candidates should be eliminated when no X-Wing pattern exists
        var targetCell = result.GetCell(2, 2)!;
        targetCell.GetPossibleValues().Should().BeEquivalentTo(originalPossibleValues);
    }
} 