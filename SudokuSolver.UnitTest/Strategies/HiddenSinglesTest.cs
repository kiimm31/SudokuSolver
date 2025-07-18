using FluentAssertions;
using SudokuSolver.Core.Strategies;
using SudokuSolver.Domain.Models;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest.Strategies;

public class HiddenSinglesTest : BaseTest
{
    [Test]
    public void HiddenSingles()
    {
        var cells = new List<Cell>
        {
            GenerateCellWithPossibleValues(1, 1, [1, 2, 3, 4, 5, 6, 7, 8, 9]),
            GenerateCellWithPossibleValues(1, 2, [2, 3, 4, 5, 6, 7, 8, 9]),
            GenerateCellWithPossibleValues(1, 3, [2, 3, 4, 5, 6, 7, 8, 9]),
            GenerateCellWithPossibleValues(1, 4, [2, 3, 4, 5, 6, 7, 8, 9]),
            GenerateCellWithPossibleValues(1, 5, [2, 3, 4, 5, 6, 7, 8, 9]),
            GenerateCellWithPossibleValues(1, 6, [2, 3, 4, 5, 6, 7, 8, 9]),
            GenerateCellWithPossibleValues(1, 7, [2, 3, 4, 5, 6, 7, 8, 9]),
            GenerateCellWithPossibleValues(1, 8, [2, 3, 4, 5, 6, 7, 8, 9]),
            GenerateCellWithPossibleValues(1, 9, [2, 3, 4, 5, 6, 7, 8, 9]),
        };
        
        var grid = new Grid(cells);

        var solve = new HiddenSinglesStrategy().Apply(grid);
        var cell = solve.GetCell(1, 1)!;
        cell.IsConfirmed.Should().BeTrue();
        cell.IsSolved.Should().BeTrue();
        cell.Value.Should().Be(1);
    }
}