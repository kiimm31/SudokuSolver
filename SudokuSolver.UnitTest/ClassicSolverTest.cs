using FluentAssertions;
using SudokuSolver.Core.Factories;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest;

public class ClassicSolverTest : BaseTest
{
    [Test]
    public void Hardest()
    {
        var gridRaw = new int[9, 9]
        {
            { 5, 6, 0,/**/ 0, 8, 0,/**/ 0, 0, 0 },
            { 0, 4, 8,/**/ 0, 0, 0,/**/ 0, 0, 6 },
            { 0, 0, 0,/**/ 0, 6, 4,/**/ 0, 2, 0 },
            /*--------------------------*/
            { 9, 0, 0,/**/ 1, 0, 0,/**/ 6, 0, 2 },
            { 4, 0, 6,/**/ 0, 0, 3,/**/ 0, 0, 1 },
            { 0, 1, 0,/**/ 5, 9, 6,/**/ 0, 3, 0 },
            /*--------------------------*/
            { 3, 0, 0,/**/ 0, 0, 0,/**/ 0, 6, 0 },
            { 0, 7, 0,/**/ 6, 0, 0,/**/ 0, 0, 5 },
            { 6, 0, 0,/**/ 0, 3, 9,/**/ 0, 0, 0 }
        };

        // Convert the raw grid to a Grid object
        var grid = GenerateGrid(gridRaw);

        var solver = SudokuSolverFactory.CreateClassicSolver(grid);

        // Test that the solver can solve this easy puzzle
        var result = solver.Solve();

        Console.Write(result.ToString());

        result.IsSolved().Should().BeTrue();
    }

    [Test]
    public void PrintGrid()
    {
        var gridRaw = new int[9, 9]
        {
            { 5, 3, 0, 0, 7, 0, 0, 0, 0 },
            { 6, 0, 0, 1, 9, 5, 0, 0, 0 },
            { 0, 9, 8, 0, 0, 0, 0, 6, 0 },
            { 8, 0, 0, 0, 6, 0, 0, 0, 3 },
            { 4, 0, 0, 8, 0, 3, 0, 0, 1 },
            { 7, 0, 0, 0, 2, 0, 0, 0, 6 },
            { 0, 6, 0, 0, 0, 0, 2, 8, 0 },
            { 0, 0, 0, 4, 1, 9, 0, 0, 5 },
            { 0, 0, 0, 0, 8, 0, 0, 7, 9 }
        };

        var grid = GenerateGrid(gridRaw);

        var printGrid = grid.ToString();

        printGrid.Should().NotBeNullOrEmpty();

        printGrid.Should().BeEquivalentTo(
            "5 3 . | . 7 . | . . . | \r\n6 . . | 1 9 5 | . . . | \r\n. 9 8 | . . . | . 6 . | \r\n------+-------+------\r\n8 . . | . 6 . | . . 3 | \r\n4 . . | 8 . 3 | . . 1 | \r\n7 . . | . 2 . | . . 6 | \r\n------+-------+------\r\n. 6 . | . . . | 2 8 . | \r\n. . . | 4 1 9 | . . 5 | \r\n. . . | . 8 . | . 7 9 | \r\n");
        grid.IsSolved().Should().BeFalse();
    }

    [Test]
    public void GivenStuck_ShouldBreak()
    {
        var gridRaw = new int[9, 9]
        {
            { 1,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0 }
        };

        var grid = GenerateGrid(gridRaw);

        var solver = SudokuSolverFactory.CreateClassicSolver(grid);
        
        var result = solver.Solve();

        result.IsSolved().Should().BeFalse();
    }
}