using FluentAssertions;
using SudokuSolver.Core.Strategies;
using SudokuSolver.Domain.Models;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest.Strategies;

public class XYWingStrategyTest : BaseTest
{
    [Test]
    public void XYWing_Should_Eliminate_Candidates_From_Common_Cells()
    {
        // Arrange: Create a grid with XY-Wing pattern
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
        
        // Set up XY-Wing pattern:
        // Pivot cell (1,1) has candidates [1,2] (XY)
        // Pincer cell 1 (1,3) has candidates [1,3] (XZ)
        // Pincer cell 2 (3,1) has candidates [2,3] (YZ)
        // Common value Z=3 should be eliminated from cells that see both pincers
        
        // Pivot cell (1,1) - candidates [1,2]
        grid.GetCell(1, 1)!.EliminatePossibleValue(3);
        grid.GetCell(1, 1)!.EliminatePossibleValue(4);
        grid.GetCell(1, 1)!.EliminatePossibleValue(5);
        grid.GetCell(1, 1)!.EliminatePossibleValue(6);
        grid.GetCell(1, 1)!.EliminatePossibleValue(7);
        grid.GetCell(1, 1)!.EliminatePossibleValue(8);
        grid.GetCell(1, 1)!.EliminatePossibleValue(9);
        
        // Pincer cell 1 (1,3) - candidates [1,3]
        grid.GetCell(1, 3)!.EliminatePossibleValue(2);
        grid.GetCell(1, 3)!.EliminatePossibleValue(4);
        grid.GetCell(1, 3)!.EliminatePossibleValue(5);
        grid.GetCell(1, 3)!.EliminatePossibleValue(6);
        grid.GetCell(1, 3)!.EliminatePossibleValue(7);
        grid.GetCell(1, 3)!.EliminatePossibleValue(8);
        grid.GetCell(1, 3)!.EliminatePossibleValue(9);
        
        // Pincer cell 2 (3,1) - candidates [2,3]
        grid.GetCell(3, 1)!.EliminatePossibleValue(1);
        grid.GetCell(3, 1)!.EliminatePossibleValue(4);
        grid.GetCell(3, 1)!.EliminatePossibleValue(5);
        grid.GetCell(3, 1)!.EliminatePossibleValue(6);
        grid.GetCell(3, 1)!.EliminatePossibleValue(7);
        grid.GetCell(3, 1)!.EliminatePossibleValue(8);
        grid.GetCell(3, 1)!.EliminatePossibleValue(9);
        
        // Add a cell that sees both pincers and has values 3,4 as candidates (should have 3 eliminated)
        grid.GetCell(3, 3)!.EliminatePossibleValue(1);
        grid.GetCell(3, 3)!.EliminatePossibleValue(2);
        grid.GetCell(3, 3)!.EliminatePossibleValue(5);
        grid.GetCell(3, 3)!.EliminatePossibleValue(6);
        grid.GetCell(3, 3)!.EliminatePossibleValue(7);
        grid.GetCell(3, 3)!.EliminatePossibleValue(8);
        grid.GetCell(3, 3)!.EliminatePossibleValue(9);
        
        // Act: Apply XY-Wing strategy
        var strategy = new XYWingStrategy();
        var result = strategy.Apply(grid);
        
        // Assert: Value 3 should be eliminated from cell (3,3)
        var targetCell = result.GetCell(3, 3)!;
        targetCell.GetPossibleValues().Should().NotContain(3);
    }
    
    [Test]
    public void XYWing_Should_Not_Eliminate_When_No_XYWing_Pattern_Exists()
    {
        // Arrange: Create a grid without XY-Wing pattern
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
        
        // Act: Apply XY-Wing strategy
        var strategy = new XYWingStrategy();
        var result = strategy.Apply(grid);
        
        // Assert: No candidates should be eliminated when no XY-Wing pattern exists
        var targetCell = result.GetCell(2, 2)!;
        targetCell.GetPossibleValues().Should().BeEquivalentTo(originalPossibleValues);
    }
    
    [Test]
    public void XYWing_Should_Not_Eliminate_When_Cells_Do_Not_See_Both_Pincers()
    {
        // Arrange: Create a grid with XY-Wing pattern but cell doesn't see both pincers
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
        
        // Set up XY-Wing pattern:
        // Pivot cell (1,1) has candidates [1,2] (XY)
        // Pincer cell 1 (1,3) has candidates [1,3] (XZ)
        // Pincer cell 2 (3,1) has candidates [2,3] (YZ)
        
        // Pivot cell (1,1) - candidates [1,2]
        grid.GetCell(1, 1)!.EliminatePossibleValue(3);
        grid.GetCell(1, 1)!.EliminatePossibleValue(4);
        grid.GetCell(1, 1)!.EliminatePossibleValue(5);
        grid.GetCell(1, 1)!.EliminatePossibleValue(6);
        grid.GetCell(1, 1)!.EliminatePossibleValue(7);
        grid.GetCell(1, 1)!.EliminatePossibleValue(8);
        grid.GetCell(1, 1)!.EliminatePossibleValue(9);
        
        // Pincer cell 1 (1,3) - candidates [1,3]
        grid.GetCell(1, 3)!.EliminatePossibleValue(2);
        grid.GetCell(1, 3)!.EliminatePossibleValue(4);
        grid.GetCell(1, 3)!.EliminatePossibleValue(5);
        grid.GetCell(1, 3)!.EliminatePossibleValue(6);
        grid.GetCell(1, 3)!.EliminatePossibleValue(7);
        grid.GetCell(1, 3)!.EliminatePossibleValue(8);
        grid.GetCell(1, 3)!.EliminatePossibleValue(9);
        
        // Pincer cell 2 (3,1) - candidates [2,3]
        grid.GetCell(3, 1)!.EliminatePossibleValue(1);
        grid.GetCell(3, 1)!.EliminatePossibleValue(4);
        grid.GetCell(3, 1)!.EliminatePossibleValue(5);
        grid.GetCell(3, 1)!.EliminatePossibleValue(6);
        grid.GetCell(3, 1)!.EliminatePossibleValue(7);
        grid.GetCell(3, 1)!.EliminatePossibleValue(8);
        grid.GetCell(3, 1)!.EliminatePossibleValue(9);
        
        // Add a cell that only sees one pincer and has value 3 as a candidate (should NOT be eliminated)
        grid.GetCell(9, 9)!.EliminatePossibleValue(1);
        grid.GetCell(9, 9)!.EliminatePossibleValue(2);
        grid.GetCell(9, 9)!.EliminatePossibleValue(4);
        grid.GetCell(9, 9)!.EliminatePossibleValue(5);
        grid.GetCell(9, 9)!.EliminatePossibleValue(6);
        grid.GetCell(9, 9)!.EliminatePossibleValue(7);
        grid.GetCell(9, 9)!.EliminatePossibleValue(8);
        grid.GetCell(9, 9)!.EliminatePossibleValue(9);
        
        // Act: Apply XY-Wing strategy
        var strategy = new XYWingStrategy();
        var result = strategy.Apply(grid);
        
        // Assert: Value 3 should NOT be eliminated from cell (9,9) since it doesn't see both pincers
        var targetCell = result.GetCell(9, 9)!;
        targetCell.GetPossibleValues().Should().Contain(3);
    }
} 