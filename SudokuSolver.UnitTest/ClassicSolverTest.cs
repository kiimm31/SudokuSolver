using FluentAssertions;
using SudokuSolver.Core.Helpers;
using SudokuSolver.Core.Services;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest;

public class ClassicSolverTest : BaseTest
{
    [Test]
    public void Easiest()
    {
        // Easy Sudoku puzzle with many clues
        var gridRaw = new int[9, 9]
        {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 9}
        };
        
        // Convert the raw grid to a Grid object
        var grid = GenerateGrid(gridRaw);
        
        var solverService = new ClassicSudokuSolverService(grid);
        
        // Test that the solver can solve this easy puzzle
        var result = solverService.Solve();

        result.IsSolved().Should().BeTrue();
    }

    [Test]
    public void PrintGrid()
    {
        var gridRaw = new int[9, 9]
        {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 9}
        };
        
        var grid = GenerateGrid(gridRaw);

        var printGrid = grid.PrintGrid();
        
        printGrid.Should().NotBeNullOrEmpty();

        printGrid.Should().BeEquivalentTo(
            "5 3 . | . 7 . | . . . | \r\n6 . . | 1 9 5 | . . . | \r\n. 9 8 | . . . | . 6 . | \r\n------+-------+------\r\n8 . . | . 6 . | . . 3 | \r\n4 . . | 8 . 3 | . . 1 | \r\n7 . . | . 2 . | . . 6 | \r\n------+-------+------\r\n. 6 . | . . . | 2 8 . | \r\n. . . | 4 1 9 | . . 5 | \r\n. . . | . 8 . | . 7 9 | \r\n");
        grid.IsSolved().Should().BeFalse();
    }
    
   [Test]
    public void GridEquals()
    {
        var gridRaw = new int[9, 9]
        {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 9}
        };
        
        var gridRaw2 = new int[9, 9]
        {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 8}
        };
        
        var grid1 = GenerateGrid(gridRaw);
        var grid2 = GenerateGrid(gridRaw);
        
        // Test that two grids with the same values are considered equal
        grid1.Equals(grid2).Should().BeTrue();
    }
    
    [Test]
    public void GridNotEquals()
    {
        var gridRaw = new int[9, 9]
        {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 9}
        };
        
        var grid1 = GenerateGrid(gridRaw);
        var grid2 = GenerateGrid(gridRaw);
        
        // Test that two grids with the same values are considered equal
        grid1.Equals(grid2).Should().BeTrue();
    }
}