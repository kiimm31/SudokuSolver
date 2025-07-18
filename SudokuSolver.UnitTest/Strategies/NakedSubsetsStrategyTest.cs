using FluentAssertions;
using SudokuSolver.Core.Strategies;
using SudokuSolver.Domain.Models;
using SudokuSolver.UnitTest.Base;

namespace SudokuSolver.UnitTest.Strategies;

public class NakedSubsetsStrategyTest : BaseTest
{
    private NakedSubsetsStrategy _pairStrategy;

    [SetUp]
    public void Setup()
    {
        _pairStrategy = new NakedSubsetsStrategy();
    }

    [Test]
    public void RowPairStrategyTest()
    {
        var rowCells = new List<Cell>
        {
            GenerateCellWithPossibleValues(1, 1, [1, 2]),
            GenerateCellWithPossibleValues(1, 2, [1, 2]),
            GenerateCellWithPossibleValues(1, 3, [1, 2, 3, 4]),
            GenerateCellWithPossibleValues(1, 4, [1, 2, 5, 6]),
        };

        var grid = new Grid(rowCells);

        var result = _pairStrategy.Apply(grid).GetRow(1);

        result.Should().BeEquivalentTo([
            GenerateCellWithPossibleValues(1, 1, [1, 2]),
            GenerateCellWithPossibleValues(1, 2, [1, 2]),
            GenerateCellWithPossibleValues(1, 3, [3, 4]),
            GenerateCellWithPossibleValues(1, 4, [5, 6])]);
    }
    
    [Test]
    public void ColumnPairStrategyTest()
    {
        var columnCells = new List<Cell>
        {
            GenerateCellWithPossibleValues(1, 1, [1, 2]),
            GenerateCellWithPossibleValues(2, 1, [1, 2]),
            GenerateCellWithPossibleValues(3, 1, [1, 2, 3, 4]),
            GenerateCellWithPossibleValues(4, 1, [1, 2, 5, 6]),
        };

        var grid = new Grid(columnCells);

        var result = _pairStrategy.Apply(grid).GetColumn(1);

        result.Should().BeEquivalentTo([
            GenerateCellWithPossibleValues(1, 1, [1, 2]),
            GenerateCellWithPossibleValues(2, 1, [1, 2]),
            GenerateCellWithPossibleValues(3, 1, [3, 4]),
            GenerateCellWithPossibleValues(4, 1, [5, 6])]);
    }
    
    [Test]
    public void BoxPairStrategyTest()
    {
        var boxCells = new List<Cell>
        {
            GenerateCellWithPossibleValues(1, 1, [1, 2]),
            GenerateCellWithPossibleValues(1, 2, [1, 2]),
            GenerateCellWithPossibleValues(2, 1, [1, 2, 3, 4]),
            GenerateCellWithPossibleValues(2, 2, [1, 2, 5, 6]),
        };

        var grid = new Grid(boxCells);

        var result = _pairStrategy.Apply(grid).GetBox(1, 1);

        result.Should().BeEquivalentTo([
            GenerateCellWithPossibleValues(1, 1, [1, 2]),
            GenerateCellWithPossibleValues(1, 2, [1, 2]),
            GenerateCellWithPossibleValues(2, 1, [3, 4]),
            GenerateCellWithPossibleValues(2, 2, [5, 6])]);
    }
}