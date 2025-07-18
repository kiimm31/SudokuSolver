using FluentAssertions;
using SudokuSolver.Core.Strategies;
using SudokuSolver.Domain.Models;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest.Strategies;

public class SwordfishStrategyTest : BaseTest
{
    [Test]
    public void Swordfish_Should_Eliminate_Candidates_From_Columns()
    {
        // Arrange: Create a grid with Swordfish pattern in columns 1, 3, and 5
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
        
        // Set up Swordfish pattern: value 6 appears only in rows 1,3,5 and columns 1,3,5
        // Row 1: value 6 only in columns 1, 3, 5
        grid.GetCell(1, 1)!.EliminatePossibleValue(2);
        grid.GetCell(1, 1)!.EliminatePossibleValue(3);
        grid.GetCell(1, 1)!.EliminatePossibleValue(4);
        grid.GetCell(1, 1)!.EliminatePossibleValue(7);
        grid.GetCell(1, 1)!.EliminatePossibleValue(8);
        grid.GetCell(1, 1)!.EliminatePossibleValue(9);
        
        grid.GetCell(1, 3)!.EliminatePossibleValue(2);
        grid.GetCell(1, 3)!.EliminatePossibleValue(3);
        grid.GetCell(1, 3)!.EliminatePossibleValue(4);
        grid.GetCell(1, 3)!.EliminatePossibleValue(7);
        grid.GetCell(1, 3)!.EliminatePossibleValue(8);
        grid.GetCell(1, 3)!.EliminatePossibleValue(9);
        
        grid.GetCell(1, 5)!.EliminatePossibleValue(2);
        grid.GetCell(1, 5)!.EliminatePossibleValue(3);
        grid.GetCell(1, 5)!.EliminatePossibleValue(4);
        grid.GetCell(1, 5)!.EliminatePossibleValue(7);
        grid.GetCell(1, 5)!.EliminatePossibleValue(8);
        grid.GetCell(1, 5)!.EliminatePossibleValue(9);
        
        // Row 3: value 6 only in columns 1, 3, 5
        grid.GetCell(3, 1)!.EliminatePossibleValue(2);
        grid.GetCell(3, 1)!.EliminatePossibleValue(3);
        grid.GetCell(3, 1)!.EliminatePossibleValue(4);
        grid.GetCell(3, 1)!.EliminatePossibleValue(7);
        grid.GetCell(3, 1)!.EliminatePossibleValue(8);
        grid.GetCell(3, 1)!.EliminatePossibleValue(9);
        
        grid.GetCell(3, 3)!.EliminatePossibleValue(2);
        grid.GetCell(3, 3)!.EliminatePossibleValue(3);
        grid.GetCell(3, 3)!.EliminatePossibleValue(4);
        grid.GetCell(3, 3)!.EliminatePossibleValue(7);
        grid.GetCell(3, 3)!.EliminatePossibleValue(8);
        grid.GetCell(3, 3)!.EliminatePossibleValue(9);
        
        grid.GetCell(3, 5)!.EliminatePossibleValue(2);
        grid.GetCell(3, 5)!.EliminatePossibleValue(3);
        grid.GetCell(3, 5)!.EliminatePossibleValue(4);
        grid.GetCell(3, 5)!.EliminatePossibleValue(7);
        grid.GetCell(3, 5)!.EliminatePossibleValue(8);
        grid.GetCell(3, 5)!.EliminatePossibleValue(9);
        
        // Row 5: value 6 only in columns 1, 3, 5
        grid.GetCell(5, 1)!.EliminatePossibleValue(2);
        grid.GetCell(5, 1)!.EliminatePossibleValue(3);
        grid.GetCell(5, 1)!.EliminatePossibleValue(4);
        grid.GetCell(5, 1)!.EliminatePossibleValue(7);
        grid.GetCell(5, 1)!.EliminatePossibleValue(8);
        grid.GetCell(5, 1)!.EliminatePossibleValue(9);
        
        grid.GetCell(5, 3)!.EliminatePossibleValue(2);
        grid.GetCell(5, 3)!.EliminatePossibleValue(3);
        grid.GetCell(5, 3)!.EliminatePossibleValue(4);
        grid.GetCell(5, 3)!.EliminatePossibleValue(7);
        grid.GetCell(5, 3)!.EliminatePossibleValue(8);
        grid.GetCell(5, 3)!.EliminatePossibleValue(9);
        
        grid.GetCell(5, 5)!.EliminatePossibleValue(2);
        grid.GetCell(5, 5)!.EliminatePossibleValue(3);
        grid.GetCell(5, 5)!.EliminatePossibleValue(4);
        grid.GetCell(5, 5)!.EliminatePossibleValue(7);
        grid.GetCell(5, 5)!.EliminatePossibleValue(8);
        grid.GetCell(5, 5)!.EliminatePossibleValue(9);
        
        // Remove value 6 from other cells in rows 1, 3, 5 (except columns 1, 3, 5)
        for (int col = 1; col <= 9; col++)
        {
            if (col != 1 && col != 3 && col != 5)
            {
                grid.GetCell(1, col)!.EliminatePossibleValue(6);
                grid.GetCell(3, col)!.EliminatePossibleValue(6);
                grid.GetCell(5, col)!.EliminatePossibleValue(6);
            }
        }
        
        // Remove value 6 from other cells in columns 1, 3, 5 (except rows 1, 3, 5)
        for (int row = 1; row <= 9; row++)
        {
            if (row != 1 && row != 3 && row != 5)
            {
                grid.GetCell(row, 1)!.EliminatePossibleValue(6);
                grid.GetCell(row, 3)!.EliminatePossibleValue(6);
                grid.GetCell(row, 5)!.EliminatePossibleValue(6);
            }
        }
        
        // Add a cell in column 1 that has value 6 as a candidate (should be eliminated)
        grid.GetCell(2, 1)!.EliminatePossibleValue(2);
        grid.GetCell(2, 1)!.EliminatePossibleValue(3);
        grid.GetCell(2, 1)!.EliminatePossibleValue(4);
        grid.GetCell(2, 1)!.EliminatePossibleValue(7);
        grid.GetCell(2, 1)!.EliminatePossibleValue(8);
        grid.GetCell(2, 1)!.EliminatePossibleValue(9);
        
        // Act: Apply Swordfish strategy
        var strategy = new SwordfishStrategy();
        var result = strategy.Apply(grid);
        
        // Assert: Value 6 should be eliminated from cell (2,1)
        var targetCell = result.GetCell(2, 1)!;
        targetCell.GetPossibleValues().Should().NotContain(6);
    }
    
    [Test]
    public void Swordfish_Should_Eliminate_Candidates_From_Rows()
    {
        // Arrange: Create a grid with Swordfish pattern in rows 1, 3, and 5
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
        
        // Set up Swordfish pattern: value 8 appears only in columns 1,3,5 and rows 1,3,5
        // Column 1: value 8 only in rows 1, 3, 5
        grid.GetCell(1, 1)!.EliminatePossibleValue(2);
        grid.GetCell(1, 1)!.EliminatePossibleValue(3);
        grid.GetCell(1, 1)!.EliminatePossibleValue(4);
        grid.GetCell(1, 1)!.EliminatePossibleValue(5);
        grid.GetCell(1, 1)!.EliminatePossibleValue(6);
        grid.GetCell(1, 1)!.EliminatePossibleValue(7);
        grid.GetCell(1, 1)!.EliminatePossibleValue(9);
        
        grid.GetCell(3, 1)!.EliminatePossibleValue(2);
        grid.GetCell(3, 1)!.EliminatePossibleValue(3);
        grid.GetCell(3, 1)!.EliminatePossibleValue(4);
        grid.GetCell(3, 1)!.EliminatePossibleValue(5);
        grid.GetCell(3, 1)!.EliminatePossibleValue(6);
        grid.GetCell(3, 1)!.EliminatePossibleValue(7);
        grid.GetCell(3, 1)!.EliminatePossibleValue(9);
        
        grid.GetCell(5, 1)!.EliminatePossibleValue(2);
        grid.GetCell(5, 1)!.EliminatePossibleValue(3);
        grid.GetCell(5, 1)!.EliminatePossibleValue(4);
        grid.GetCell(5, 1)!.EliminatePossibleValue(5);
        grid.GetCell(5, 1)!.EliminatePossibleValue(6);
        grid.GetCell(5, 1)!.EliminatePossibleValue(7);
        grid.GetCell(5, 1)!.EliminatePossibleValue(9);
        
        // Column 3: value 8 only in rows 1, 3, 5
        grid.GetCell(1, 3)!.EliminatePossibleValue(2);
        grid.GetCell(1, 3)!.EliminatePossibleValue(3);
        grid.GetCell(1, 3)!.EliminatePossibleValue(4);
        grid.GetCell(1, 3)!.EliminatePossibleValue(5);
        grid.GetCell(1, 3)!.EliminatePossibleValue(6);
        grid.GetCell(1, 3)!.EliminatePossibleValue(7);
        grid.GetCell(1, 3)!.EliminatePossibleValue(9);
        
        grid.GetCell(3, 3)!.EliminatePossibleValue(2);
        grid.GetCell(3, 3)!.EliminatePossibleValue(3);
        grid.GetCell(3, 3)!.EliminatePossibleValue(4);
        grid.GetCell(3, 3)!.EliminatePossibleValue(5);
        grid.GetCell(3, 3)!.EliminatePossibleValue(6);
        grid.GetCell(3, 3)!.EliminatePossibleValue(7);
        grid.GetCell(3, 3)!.EliminatePossibleValue(9);
        
        grid.GetCell(5, 3)!.EliminatePossibleValue(2);
        grid.GetCell(5, 3)!.EliminatePossibleValue(3);
        grid.GetCell(5, 3)!.EliminatePossibleValue(4);
        grid.GetCell(5, 3)!.EliminatePossibleValue(5);
        grid.GetCell(5, 3)!.EliminatePossibleValue(6);
        grid.GetCell(5, 3)!.EliminatePossibleValue(7);
        grid.GetCell(5, 3)!.EliminatePossibleValue(9);
        
        // Column 5: value 8 only in rows 1, 3, 5
        grid.GetCell(1, 5)!.EliminatePossibleValue(2);
        grid.GetCell(1, 5)!.EliminatePossibleValue(3);
        grid.GetCell(1, 5)!.EliminatePossibleValue(4);
        grid.GetCell(1, 5)!.EliminatePossibleValue(5);
        grid.GetCell(1, 5)!.EliminatePossibleValue(6);
        grid.GetCell(1, 5)!.EliminatePossibleValue(7);
        grid.GetCell(1, 5)!.EliminatePossibleValue(9);
        
        grid.GetCell(3, 5)!.EliminatePossibleValue(2);
        grid.GetCell(3, 5)!.EliminatePossibleValue(3);
        grid.GetCell(3, 5)!.EliminatePossibleValue(4);
        grid.GetCell(3, 5)!.EliminatePossibleValue(5);
        grid.GetCell(3, 5)!.EliminatePossibleValue(6);
        grid.GetCell(3, 5)!.EliminatePossibleValue(7);
        grid.GetCell(3, 5)!.EliminatePossibleValue(9);
        
        grid.GetCell(5, 5)!.EliminatePossibleValue(2);
        grid.GetCell(5, 5)!.EliminatePossibleValue(3);
        grid.GetCell(5, 5)!.EliminatePossibleValue(4);
        grid.GetCell(5, 5)!.EliminatePossibleValue(5);
        grid.GetCell(5, 5)!.EliminatePossibleValue(6);
        grid.GetCell(5, 5)!.EliminatePossibleValue(7);
        grid.GetCell(5, 5)!.EliminatePossibleValue(9);
        
        // Remove value 8 from other cells in columns 1, 3, 5 (except rows 1, 3, 5)
        for (int row = 1; row <= 9; row++)
        {
            if (row != 1 && row != 3 && row != 5)
            {
                grid.GetCell(row, 1)!.EliminatePossibleValue(8);
                grid.GetCell(row, 3)!.EliminatePossibleValue(8);
                grid.GetCell(row, 5)!.EliminatePossibleValue(8);
            }
        }
        
        // Remove value 8 from other cells in rows 1, 3, 5 (except columns 1, 3, 5)
        for (int col = 1; col <= 9; col++)
        {
            if (col != 1 && col != 3 && col != 5)
            {
                grid.GetCell(1, col)!.EliminatePossibleValue(8);
                grid.GetCell(3, col)!.EliminatePossibleValue(8);
                grid.GetCell(5, col)!.EliminatePossibleValue(8);
            }
        }
        
        // Add a cell in row 2 that has value 8 as a candidate (should be eliminated)
        grid.GetCell(2, 1)!.EliminatePossibleValue(2);
        grid.GetCell(2, 1)!.EliminatePossibleValue(3);
        grid.GetCell(2, 1)!.EliminatePossibleValue(4);
        grid.GetCell(2, 1)!.EliminatePossibleValue(5);
        grid.GetCell(2, 1)!.EliminatePossibleValue(6);
        grid.GetCell(2, 1)!.EliminatePossibleValue(7);
        grid.GetCell(2, 1)!.EliminatePossibleValue(9);
        
        // Act: Apply Swordfish strategy
        var strategy = new SwordfishStrategy();
        var result = strategy.Apply(grid);
        
        // Assert: Value 8 should be eliminated from cell (2,1)
        var targetCell = result.GetCell(2, 1)!;
        targetCell.GetPossibleValues().Should().NotContain(8);
    }
    
    [Test]
    public void Swordfish_Should_Not_Eliminate_When_No_Swordfish_Pattern_Exists()
    {
        // Arrange: Create a grid without Swordfish pattern
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
        
        // Act: Apply Swordfish strategy
        var strategy = new SwordfishStrategy();
        var result = strategy.Apply(grid);
        
        // Assert: No candidates should be eliminated when no Swordfish pattern exists
        var targetCell = result.GetCell(2, 2)!;
        targetCell.GetPossibleValues().Should().BeEquivalentTo(originalPossibleValues);
    }
} 